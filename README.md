# OrderFlowComplete V2

Indicador de Order Flow / Footprint para NinjaTrader 8.

## Instalación

1. Copiar a: `Documents\NinjaTrader 8\bin\Custom\Indicators\`
2. Tools → Edit NinjaScript → Compile
3. Indicators → OrderFlowComplete V2
4. Chart: Calculate = OnEachTick

---

## Guía Visual (ordenada alfabéticamente)

### Colores de Fondo

- **Amarillo** — POC (Point of Control): el nivel con mayor volumen total de la vela. Siempre al 85% de opacidad
- **Azul** — nivel normal, sin imbalance. Más azul = más actividad en ese precio
- **Rojo** — imbalance Bid: el Bid de ese nivel es ≥ 2x el Ask del nivel de abajo. Indica vendedores agresivos dominando
- **Verde** — imbalance Ask: el Ask de ese nivel es ≥ 2x el Bid del nivel de arriba. Indica compradores agresivos dominando

### Color del Texto

- **Blanco** cuando el color de fondo es intenso (alpha ≥ 0.45) — celda oscura
- **Negro** cuando el color de fondo es tenue (alpha < 0.45) — celda clara

### Color de la Vela

- **Borde verde** si el delta total de la vela es positivo (más compras que ventas)
- **Borde rojo** si el delta es negativo

### Intensidad del Color

El nivel con más volumen (POC) es el más intenso. Los niveles con poco volumen se ven casi blancos.

En resumen visualmente:
- Una vela con mucha actividad concentrada en pocos niveles tendrá pocas celdas muy intensas y el resto casi blancas
- Una vela con volumen distribuido uniformemente tendrá todas las celdas con un color suave similar

---

## Triángulo de Divergencia

El triángulo aparece cuando se detecta una divergencia entre el precio y el delta:

- La vela cerró arriba (bullish) pero el delta fue negativo (más ventas que compras)

Es decir, el precio subió pero los vendedores agresivos dominaron el volumen. Eso sugiere que la subida no tuvo respaldo real de compradores — el precio subió "de rebote" o por falta de vendedores en esos niveles, no porque hubiera demanda agresiva.

### Interpretación en trading

- Señal de debilidad alcista
- Posible agotamiento del movimiento
- Los vendedores están absorbiendo la subida

No es una señal de entrada por sí sola, pero combinado con el contexto (resistencia, POC, imbalances rojos encima) puede indicar que la vela alcista es una trampa y el precio tiene probabilidad de girarse a la baja.

---

## Parámetros

| Parámetro | Default | Descripción |
|-----------|---------|-------------|
| Umbral imbalance pct | 200 | Porcentaje para detectar imbalances (default 2x) |
| Mostrar Delta | true | Muestra el delta de cada barra |
| Colorear velas | true | Colorea el borde de las velas según el delta |
| Mostrar CVD | true | Muestra el Cumulative Volume Delta |
| Fuente px | 11 | Tamaño de fuente |
| Ancho celda px | 100 | Ancho de cada celda |
| Alto celda px | 18 | Alto de cada celda |
| Ticks por fila | 4 | Número de ticks por cada nivel (para MNQ: 4 ticks = 1 punto) |
