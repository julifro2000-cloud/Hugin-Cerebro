## 5. Patrones de Órdenes ATM y Lecciones Aprendidas

> [!IMPORTANT]
> Esta sección recoge errores críticos y soluciones encontradas al desarrollar estrategias NinjaTrader 8 con órdenes ATM programáticas.

### 5.1 `AtmStrategyCreate` — Firma Correcta
```csharp
AtmStrategyCreate(
    OrderAction action,        // Buy / SellShort
    OrderType type,            // Limit / StopLimit / StopMarket / Market
    double limitPrice,         // Precio límite (3er argumento)
    double stopPrice,          // Precio stop (4to argumento)
    TimeInForce tif,           // Day / Gtc
    string orderId,            // GetAtmStrategyUniqueId()
    string templateName,       // Nombre exacto de la plantilla ATM
    string strategyId,         // GetAtmStrategyUniqueId() (otro distinto)
    Action<ErrorCode, string> callback  // (err, id) => {}
);
```

### 5.2 Manejo de Errores y Tipos de Orden
*   **Cantidad**: No se pasa por código; se define en la plantilla ATM de NT8.
*   **Validación de Tipo**: 
    *   **LONG**: Si entrada ≤ Close[0] → `OrderType.Limit` (pullback); si entrada > Close[0] → `OrderType.StopLimit` (rotura).
    *   **SHORT**: Si entrada ≥ Close[0] → `OrderType.Limit` (pullback); si entrada < Close[0] → `OrderType.StopLimit` (rotura).
*   **Error Callback**: Usar firma `(err, id) => {}` para evitar errores de compilación `CS1503`.

### 5.3 Arquitectura de 3 Fases (Hugin-Pattern)
Para estrategias de rotura con riesgo controlado:
1.  **Confirmación**: Cierre de vela por encima/debajo de nivel previo. Guarda un trigger (High[1]+2 / Low[1]-2).
2.  **Trigger**: En tiempo real, cuando el precio toca el trigger, calcula Stop y Entrada basada en riesgo fijo (ej. 20 ticks).
3.  **Trailing**: Sigue el mercado (mejorando precio) siempre que la distancia al swing stop sea ≤ riesgo máximo.

### 5.4 Cálculos de Riesgo Fijo
*   **Long**: `Stop = Min swing - offset`, `Entrada = Stop + RiesgoTicks`.
*   **Short**: `Stop = Max swing + offset`, `Entrada = Stop - RiesgoTicks`.

---
*NinjaMaster_NT8 - "La disciplina en la gestión es el beneficio."*

