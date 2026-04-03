# 🧪 Biblioteca de Lógica Hugin

Este documento detalla las funciones matemáticas y algoritmos que impulsan los setups del proyecto.

## 1. GenialLine (Ultimate Smoother)
Algoritmo de suavizado adaptativo que elimina el ruido sin introducir lag excesivo.
*   **Fórmula Base**: Proyecta una media del precio suavizada por la volatilidad (Range).
*   **Bandas**: Usa la desviación PI (3.14159) para crear un canal de "ruido aceptable".
*   **Uso**: Cruce de precio con la línea o cambio de color (Giro de tendencia).
*   **Sincronización Múltiple (Triple Genial Line)**: Se evalúa el cruce del precio contra 3 líneas simultáneamente en distintos marcos temporales (ej. 28s, 144s, 12m). Una **tendencia sincronizada** se da cuando el precio está por encima de las 3 líneas (LARGO) o por debajo de las 3 (CORTO), confirmando dirección a micro, corto y medio plazo. (Implementado en `HuginTripleGenialLineSign`).

## 2. Fib50 Retroceso (Swings Alternados)
Detector de liquidez en niveles del 50%.
*   **Lógica**: Detecta un Swing High seguido de un Swing Low (o viceversa) en la serie de 144s.
*   **Criterio de Impulso**: Solo registra un nuevo impulso si el precio recorre una distancia mínima (TickSize * 0.5).
*   **Zonas de Toque**: Crea una "zona de acción" de ±4 ticks alrededor del nivel exacto.

## 3. VixTrigger (Panic/Greed Index)
Mapeo de condiciones de clímax de mercado.
*   **Long Panic**: Identifica cuando el precio está sobrevendido y el VIX (o su proxy en el activo) dispara un trigger de reversa.
*   **Reversa**: Confirmación mediante el patrón de rotura de máximo/mínimo de la vela de clímax.

## 4. Filtro de Volumen SMA
*   **Lógica**: Compara el volumen relativo con su media de 20 periodos.
*   **Coloreado**: Cyan (Explosión de volumen), Gris (Volumen estructural).

---
*NinjaMaster_NT8 - "La lógica es la ley del mercado."*
