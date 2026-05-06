# ⚡ Optimización de Alto Rendimiento (Hugin Standard)

Este documento contiene las reglas de oro aprendidas en la implementación de la **V9** para mantener NinjaTrader 8 rápido y estable.

## I. Gestión de Memoria
*   **Structs vs Classes**: Para objetos que se crean mil veces (Swings, Impulsos), usa `struct`. Se alojan en el Stack y no generan basura para el Garbage Collector.
*   **Enums**: Nunca uses clases o strings para estados de tendencia. Usa `private enum Trend { Bull, Bear, Neutral }`.
*   **Capacidad Inicial**: Al crear una `List` o `Dictionary`, define siempre su tamaño estimado: `new List<double>(1000)`.

## II. Renderizado WPF (Gráficos)
*   **Frozen Brushes**: Los pinceles de dibujo DEBEN ser congelados.
    ```csharp
    SolidColorBrush myBrush = new SolidColorBrush(Colors.Cyan);
    myBrush.Freeze(); // Crucial para la velocidad de dibujo.
    ```
*   **PlotBrushes**: Es más eficiente usar `PlotBrushes[0][0] = color;` que llamar a `Draw.Text` o `Draw.Line` en cada tick.

## III. Algoritmia Eficiente
*   **O(1) vs O(n)**: No recorras todo el historial de la sesión para buscar un toque. Mantén un índice (`_touchStartIdx`) y busca solo en los últimos N elementos activos.
*   **Aritmética**: Usa `price * 0.5` en lugar de `price / 2.0`. El procesador multiplica más rápido de lo que divide.

---
*NinjaMaster_NT8 - "Velocidad es precisión."*
