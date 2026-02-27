# 💎 Gestión de Recursos y Ciclo de Vida Avanzado

NinjaTrader 8 es exigente con el uso de memoria. Esta guía previene los "Memory Leaks" (fugas de memoria) y cierres del programa.

## I. El Peligro de los Eventos WPF
Si añades un evento a un botón (ej. `btn.Click += MiFuncion`), **DEBES** desuscribirte en `State.Terminated`.
*   **Regla**: `btn.Click -= MiFuncion;`. Si no lo haces, NinjaTrader mantendrá el indicador en memoria para siempre, incluso después de quitarlo del gráfico.

## II. Serialización de Datos
Para que NinjaTrader guarde correctamente tus configuraciones (Brushes, Objetos):
*   Usa el patrón `[XmlIgnore]` en el objeto real y una propiedad `string` serializable para el sistema.
*   **Ejemplo**:
    ```csharp
    [XmlIgnore]
    public Brush MiColor { get; set; }
    [Browsable(false)]
    public string MiColorSerializable {
        get { return Serialize.BrushToString(MiColor); }
        set { MiColor = Serialize.StringToBrush(value); }
    }
    ```

## III. Hilos y UI
NinjaTrader procesa los datos en un hilo y la interfaz en otro.
*   Usa siempre `ChartControl.Dispatcher.InvokeAsync` para modificar elementos de la interfaz (botones, textos del HUD) desde el hilo de datos para evitar "Crashes" por acceso ilegal.

---
*NinjaMaster_NT8 - "La limpieza es la clave del rendimiento."*
