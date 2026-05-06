# 🎨 Estándares de UI/UX y Panel de Control

NinjaTrader 8 permite interfaces ricas. Aquí están los estándares para que tus indicadores y estrategias se vean "Premium".

## I. Paleta de Colores Hugin
Usamos colores con alto contraste para decisiones rápidas:
*   **Long Principal**: `Brushes.DeepSkyBlue` o `Brushes.LimeGreen`.
*   **Short Principal**: `Brushes.Red` o `Brushes.Crimson`.
*   **Alerta Temprana**: `Brushes.Orange`.
*   **Neutral / Filtro**: `Brushes.DimGray` o `Brushes.Cyan`.

## II. Integración con Chart Trader
Para estrategias automatizadas o manuales asistidas:
*   **Botones WPF**: Inserción dinámica de botones en el panel derecho del Chart Trader.
*   **HUD (Heads-Up Display)**: Overlay transparente en la esquina superior derecha con el estado de los filtros (Sincronización MTF, Tendencia, Estado ATM).

## III. Dibujo de Objetos (Best Practices)
*   **Tags Únicos**: Genera siempre tags con un contador o timestamp: `Draw.Line(this, "Tag_" + CurrentBar, ...)`.
*   **Auto-Limpieza**: Usa `IsExitOnSessionCloseStrategy = true` para limpiar objetos al final del día.

---
*NinjaMaster_NT8 - "Si no se entiende en un segundo, no sirve."*
