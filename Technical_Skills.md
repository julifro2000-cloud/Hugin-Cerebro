# 🛠️ Inventario de Habilidades Técnicas (Skills)

Este documento lista las capacidades modulares que **NinjaMaster_NT8** puede implementar, modificar o auditar en cualquier script. 

## 1. Skill: Estructuras Multi-Timeframe (MTF)
*   **Capacidad**: Sincronizar series de datos de diferentes temporalidades (ej. 1 Segundo + 144 Segundos + 28 Segundos).
*   **Mécanica**: Uso de `AddDataSeries`, `BarsInProgress` y sincronización de objetos de dibujo entre hilos.

## 2. Skill: Optimización Pro (Zero-GC)
*   **Capacidad**: Reducción drástica del lag en gráficos con muchos indicadores.
*   **Mécanica**: Implementación de `structs`, `enums`, `frozen brushes` y algoritmos de búsqueda `O(1)`.

## 3. Skill: Gestión ATM y Ejecución
*   **Capacidad**: Automatización de entrada y salida asistida.
*   **Mécanica**: Lanzamiento de órdenes `StopLimit`, gestión de IDs de ATM, trailing de entrada y lógica de `Breakeven` mediante código.

## 4. Skill: UI/UX Avanzada (Win/WPF)
*   **Capacidad**: Creación de interfaces profesionales sobre el gráfico.
*   **Mécanica**: Botones en Chart Trader, paneles HUD (Heads-Up Display), y dibujos dinámicos (Rectángulos, Triángulos, Fibracci avanzado).

## 5. Skill: Lógica de Indicadores Hugin
*   **Capacidad**: Implementación y ajuste de algoritmos propietarios.
*   **Mécanica**: `GenialLine` (Ultimate Smoother), `VixTrigger` (Pánico/Codicia), `Fib50` (Swings alternados), Filtros de Volumen media SMA.

## 6. Skill: Resiliencia y Auditoría
*   **Capacidad**: Diagnóstico y blindaje de código.
*   **Mécanica**: Try-Catch preventivo, Logs de depuración (TraceLog), y protección del `CurrentBar`.

---
*NinjaMaster_NT8 - "Identificando capacidades. Ejecución lista."*
