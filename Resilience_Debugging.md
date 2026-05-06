# 🛠️ Resiliencia, Errores y Debugging

Un código que no se puede diagnosticar es un código peligroso. Aquí están las tácticas de élite para el control de errores.

## I. Manejo Proactivo de Errores
*   **Try-Catch Táctico**: No envuelvas todo el código, solo las partes que interactúan con APIs externas o cálculos complejos (WPF, ATM, archivos).
*   **Valores Seguros**: En el `catch`, siempre asigna un valor por defecto (ej. el `Close[0]`) para que el indicador siga dibujando aunque falle el cálculo.

## II. El Arte del Log (Output Window)
*   **Formatting**: Usa `string.Format` para logs limpios:
    `Print(string.Format("[{0}] Señal detectada en {1}", Name, Time[0]));`
*   **Filtros de Log**: En producción, usa una propiedad `bool TraceLog` para activar/desactivar los mensajes de depuración y no saturar la CPU.

## III. Debugging de Estados
Si un indicador no se comporta bien al arrancar, añade logs en cada `State`:
*   `State.Configure`: Solo para series adicionales.
*   `State.DataLoaded`: Para instanciar indicadores y cargar históricos.
*   `State.Realtime`: Para resetear variables de trading antes de poner dinero en riesgo.

---
*NinjaMaster_NT8 - "Si no puedes medirlo, no puedes arreglarlo."*
