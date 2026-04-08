# 🗺️ Patrón Deepchart Overlay (Panel Único Espacial)

Este patrón establece la metodología para renderizar múltiples componentes visuales e indicadores de datos complejos (como Footprint, mapas de calor del Order Flow y Grids inferiores) dentro del panel principal de precios de forma limpia y eficiente, sin necesidad de emplear paneles secundarios. 

Implementado de manera óptima en **HuginDeltaGridV2**, este estándar se resume en los siguientes pilares de desarrollo (NinjaMaster_NT8):

## I. Panel Único con Frontera Virtual
Configuramos `IsOverlay = true`, garantizando que el método `OnRender()` sólo escale respecto al precio de los gráficos base. 
La separación de componentes se logra dividiendo matemáticamente el área de píxeles: reservando el espacio inferior para widgets, grids o tablas.

```csharp
// Definir altura de reserva inferior (ej. AlturaGridPx = 90px)
float gridTop = priceScale.GetYByValue(priceScale.MinValue) - AlturaGridPx;

// Al dibujar la parte superior del overlay, recortar si entra en gridTop
if (yMin + h > gridTop) continue; 
```

## II. Renderizado de Alto Rendimiento (SharpDX / Direct2D1)
*   **Primitive Drawing**: Se evitan comandos wrapper de NinjaTrader (`Draw.Rect`, `Draw.Text`) cuando dibujamos cientos de elementos de footprint o celdas de grid, y utilizamos el motor interno `SharpDX` (ej: `RenderTarget.FillRectangle`).
*   **Diccionarios eficientes**: Crear estructuras dedicadas agrupadas en base a `CurrentBar` limitándolas al constructor (`new Dictionary<int, BarData>(2000)`) o cargarlas previamente, para esquivar el uso del Garbage Collector intra-tick.

## III. Simulador Histórico Híbrido (`OnBarUpdate`)
Solución NinjaScript para proveer datos a indicadores avanzados que requieren `BarsType.Volumetric` aunque el usuario carezca de ellos o esté probando la estrategia en gráfico estándar:
*   **Modo Volumétrico**: Utilizar `GetBidVolumeForPrice` y `GetAskVolumeForPrice` directamente.
*   **Modo Simulado Heurístico**: Ante la ausencia del tipo, asignar ratios empíricos (Ej: 65% a favor de la vela) recorriendo el Alto/Bajo dividiéndolo por el `TickSize`.

## IV. Gestión Microestructural en Tiempo Real (`OnMarketData`)
Evaluar la agresividad sub-tick usando interpolación y fallbacks robustos:
1. Precio vs Spread (`e.Ask` / `e.Bid`).
2. Fallback "Tick Down/Up" en caso de que L1 esté momentáneamente vacío o distorsionado: si `price > _lastPrice` asume Agresor Buy (Ask Vol).

## V. Caching Visual Adaptativo y Auto-Zoon (Responsive Alpha)
Para evitar la saturación visual y lograr un "heat map" efectivo:
* Calcular el volumen dinámicamente limitándolo solo al campo visual iterando `ChartBars.FromIndex` hasta `ChartBars.ToIndex`.
* Alterar el canal `Alpha` de los recursos (`SolidColorBrush` o `Color` struct) en función del recuento global del marco o la ventana abierta.

---
*NinjaMaster_NT8 - "Integración perfecta de UI masiva en un panel único de precio."*
