# 🛠️ Estrategias, Órdenes y Gestión ATM

Guía para el desarrollo de sistemas de ejecución (`Strategies`) usando la API de NinjaTrader 8.

## 1. Entrada de Órdenes
*   **Market Orders**: Rápidas pero con slippage. Usar solo en scalping de altísima volatilidad.
*   **StopLimit**: El estándar para roturas. Define un `stopPrice` y un `limitPrice` con un pequeño offset para asegurar el llenado.

## 2. Gestión de Plantillas ATM
*   **Carga Dinámica**: Permite al usuario elegir la plantilla ATM desde el panel de propiedades mediante un `string`.
*   **AtmStrategyCreate**: El método principal para lanzar órdenes con Stop Loss y Take Profit predefinidos.

## 3. Trailing de Entrada
Técnica para mover el precio de entrada mientras el setup sigue vigente:
*   **Long**: Si el máximo de la vela previa es menor al nivel de entrada actual, bajamos el nivel de entrada para mejorar el precio.

## 4. Estado de la Estrategia
Siempre resetea IDs y variables al pasar de `Historical` a `Realtime` en `OnStateChange` para evitar que la estrategia intente cerrar posiciones que no existen.

---
*NinjaMaster_NT8 - "La disciplina en la gestión es el beneficio."*
