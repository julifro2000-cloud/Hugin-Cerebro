# 🏗️ Estándares de Arquitectura Hugin

Este documento define las reglas de construcción y mantenimiento para todos los scripts del proyecto Hugin.

## 1. Reglas de Oro de Desarrollo

### 1.1 Aprendizaje Incremental
- Todo nuevo patrón o solución debe ser documentado en este centro de conocimiento.
- El "cerebro" debe crecer con cada sesión de trabajo.

### 1.2 Optimización Obligatoria
- Los indicadores y estrategias deben ser ligeros.
- **Técnicas**: Caching de objetos (`Rectangle`, `Brush`), filtrado de `DrawObjects` por tipo, y limitación de cálculos en el hilo de UI.

### 1.3 Versionado y Seguridad
- **NUNCA** sobreescribir archivos funcionales.
- Usar sufijos de versión (ej: `V2`, `V3`).
- Mantener la compatibilidad y el historial.

## 2. Gestión de Sincronización (Git/GitHub)

### 2.1 Regla de Ramas (Visibility Rule)
- La rama local por defecto es `master`.
- La rama remota por defecto (GitHub) es **`main`**.
- Para asegurar que los cambios sean visibles al instante en la web de GitHub, las subidas deben dirigirse a `main`.
- **Comando sugerido**: `git push origin master:main`.

### 2.2 Sincronización Segura
- Antes de cualquier operación compleja de Git, realizar un commit de respaldo local (`Backup Commit`).
- Respetar siempre el archivo `.gitignore` para evitar subir archivos temporales de NinjaTrader.

---
*NinjaMaster_NT8 - "Arquitectura sólida, ejecución impecable."*
