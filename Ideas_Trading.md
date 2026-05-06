# Idea de Trading: Long Big Trades Reversión

## Descripción
Cuando hay big trades de compra (long), esperar a que el precio rompa hacia abajo la vela y supere el nivel del big trade. Luego colocar orden de venta en la caída.

## Lógica
1. Detectar big trades (volumen alto de compras)
2. Esperar vela que rompa hacia abajo del nivel del big
3. Cuando el precio supere (baje de) ese nivel, colocar orden Short
4. Stop loss: encima del máximo de la vela de ruptura
5. Take profit: riesgo 1:2 o según zona de soporte

## Condiciones
- Solo en tendencia bajista confirmada
- Contexto: precio cerca de resistencia o zona de oferta
- Confirmar con delta negativo
