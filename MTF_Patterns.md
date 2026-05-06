# 🌐 Patrones Multi-Timeframe (MTF)

Guía técnica para la proyección de datos entre diferentes marcos temporales, como la sincronización de 144s sobre 28s.

## 1. El Puente de Datos
En proyectos MTF, el indicador se ejecuta en dos hilos diferentes. Usamos una `Queue` o `Series<double>` para pasar información de la serie lenta a la rápida.

## 2. Implementación en OnStateChange
```csharp
protected override void OnStateChange() {
    if (State == State.Configure) {
        AddDataSeries(BarsPeriodType.Second, 144); // Serie secundaria
    }
}
```

## 3. Lógica de OnBarUpdate
Debes controlar siempre en qué serie estás trabajando mediante `BarsInProgress`.
*   **BarsInProgress == 1 (144s)**: Detecta patrones, señales y calcula la lógica pesada.
*   **BarsInProgress == 0 (28s)**: Dibuja los resultados y comprueba toques en tiempo real.

## 4. Sincronización de Color
No solo pases el valor del precio; proyecta también el estado visual:
`PlotBrushes[0][0] = indicador144.PlotBrushes[0][0];`

---
*NinjaMaster_NT8 - "Diferentes tiempos, misma visión."*
