#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Serialization;
using System.Windows.Media;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.NinjaScript;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
#endregion

// ══════════════════════════════════════════════════════════════════
//   ORDER FLOW COMPLETE V2 –  NinjaTrader 8  (Level 1 / Tick Rule)
//
//   Formato: una celda por nivel con texto  "BID x ASK"
//   · Fondo verde  = imbalance Ask  (Ask >= Bid * umbral)
//   · Fondo rojo   = imbalance Bid  (Bid >= Ask * umbral)
//   · Fondo amarillo = POC
//   · Fondo gris oscuro = nivel normal
//   · Delta de barra debajo en rojo/verde
//   · CVD en sub-panel
//   · Triángulo magenta en divergencia
//
//   INSTALACIÓN:
//     1. Copiar a: Documents\NinjaTrader 8\bin\Custom\Indicators\
//     2. Tools → Edit NinjaScript → Compile
//     3. Indicators → OrderFlowComplete V2
//     4. Chart: Calculate = OnEachTick
// ══════════════════════════════════════════════════════════════════

namespace NinjaTrader.NinjaScript.Indicators
{
    internal class OFLevel
    {
        public double Bid;
        public double Ask;
        public double Total => Bid + Ask;
    }

    internal class OFBar
    {
        public SortedDictionary<double, OFLevel> Levels
            = new SortedDictionary<double, OFLevel>();
        public double Delta;
        public double POC;
        public double MaxVol;
        public HashSet<double> ImbalAsk = new HashSet<double>();
        public HashSet<double> ImbalBid = new HashSet<double>();
        public bool   Divergence;
        public double OpenPx;
        public double ClosePx;
    }

    public class OrderFlowCompleteV2 : Indicator
    {
        private Dictionary<int, OFBar> _hist = new Dictionary<int, OFBar>();
        private OFBar  _cur;
        private double _lastPx;
        private bool   _lastAsk;
        private double _cvd;

        // DirectX
        private bool       _dxOk;
        private TextFormat _tfNormal;
        private TextFormat _tfBold;
        private float      _tfSize = -1f;

        private SharpDX.Direct2D1.Brush
            _bWhiteBg,    // fondo blanco base
            _bNormalBg,   // ya no usado como fondo, pero se mantiene para delta
            _bVolBg,      // azul — gradiente niveles normales
            _bAskBg,      // verde — imbalance ask
            _bBidBg,      // rojo  — imbalance bid
            _bPocBg,      // amarillo POC
            _bBorder,     // borde gris
            _bWhite,      // texto blanco (sobre celdas oscuras)
            _bDarkTxt,    // texto oscuro (sobre fondo blanco/claro)
            _bDeltaPos,   // delta verde
            _bDeltaNeg,   // delta rojo
            _bDiv;        // triángulo magenta

        // ═══════════════════════════════════════════════════════════════
        #region Propiedades
        // ═══════════════════════════════════════════════════════════════

        [NinjaScriptProperty]
        [Range(100, 500)]
        [Display(Name = "Umbral imbalance pct", GroupName = "OrderFlow", Order = 1)]
        public int ImbalPct { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Mostrar Delta", GroupName = "OrderFlow", Order = 2)]
        public bool ShowDelta { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Colorear velas", GroupName = "OrderFlow", Order = 3)]
        public bool ColorBars { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Mostrar CVD", GroupName = "OrderFlow", Order = 4)]
        public bool ShowCVD { get; set; }

        [NinjaScriptProperty]
        [Range(8, 24)]
        [Display(Name = "Fuente px", GroupName = "OrderFlow", Order = 5)]
        public int FontPx { get; set; }

        [NinjaScriptProperty]
        [Range(40, 400)]
        [Display(Name = "Ancho celda px", GroupName = "OrderFlow", Order = 6)]
        public int CellW { get; set; }

        [NinjaScriptProperty]
        [Range(10, 80)]
        [Display(Name = "Alto celda px", GroupName = "OrderFlow", Order = 7)]
        public int CellH { get; set; }

        [NinjaScriptProperty]
        [Range(1, 50)]
        [Display(Name = "Ticks por fila", GroupName = "OrderFlow", Order = 8)]
        public int TicksPerRow { get; set; }

        #endregion

        // ═══════════════════════════════════════════════════════════════
        #region OnStateChange
        // ═══════════════════════════════════════════════════════════════

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Name                     = "OrderFlowComplete V2";
                Description              = "Footprint BIDxASK · POC · Imbalances · Delta · CVD V2";
                Calculate                = Calculate.OnEachTick;
                IsOverlay                = true;
                DrawOnPricePanel         = true;
                IsAutoScale              = false;
                IsSuspendedWhileInactive = false;

                ImbalPct  = 200;
                ShowDelta = true;
                ColorBars = true;
                ShowCVD   = true;
                FontPx       = 11;
                CellW        = 100;
                CellH        = 18;
                TicksPerRow  = 4;  // MNQ: 4 ticks = 1 punto = 1 celda

                AddPlot(new Stroke(System.Windows.Media.Brushes.Cyan, 2),
                        PlotStyle.Line, "CVD");
                AddPlot(new Stroke(System.Windows.Media.Brushes.DimGray, 1),
                        PlotStyle.Line, "Zero");
            }
            else if (State == State.Terminated)
            {
                DisposeDX();
            }
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════
        #region OnMarketData — Tick Rule
        // ═══════════════════════════════════════════════════════════════

        protected override void OnMarketData(MarketDataEventArgs e)
        {
            if (e.MarketDataType != MarketDataType.Last) return;
            if (e.Volume <= 0) return;

            double px  = e.Price;
            double vol = e.Volume;

            bool isAsk;
            if      (px > _lastPx) { isAsk = true;  _lastAsk = true;  }
            else if (px < _lastPx) { isAsk = false; _lastAsk = false; }
            else                    { isAsk = _lastAsk; }
            _lastPx = px;

            if (_cur == null) _cur = new OFBar { OpenPx = px };

            // Agrupar en clusters según TicksPerRow
            // Si TicksPerRow=0 (auto), se calcula en DrawBar; aquí usamos el valor directo
            double rowSize = TickSize * Math.Max(1, TicksPerRow);
            double snap    = Math.Floor(px / rowSize) * rowSize;
            if (!_cur.Levels.ContainsKey(snap))
                _cur.Levels[snap] = new OFLevel();

            var lv = _cur.Levels[snap];
            if (isAsk) { lv.Ask += vol; _cur.Delta += vol; }
            else       { lv.Bid += vol; _cur.Delta -= vol; }

            _cur.ClosePx = px;

            if (lv.Total > _cur.MaxVol)
            { _cur.MaxVol = lv.Total; _cur.POC = snap; }
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════
        #region OnBarUpdate
        // ═══════════════════════════════════════════════════════════════

        protected override void OnBarUpdate()
        {
            if (IsFirstTickOfBar && CurrentBar > 0)
            {
                OFBar closed = _cur ?? new OFBar { OpenPx = Open[0], ClosePx = Close[0] };
                FinalizeBar(closed);
                _hist[CurrentBar - 1] = closed;
                _cvd += closed.Delta;
                _cur  = new OFBar { OpenPx = Open[0] };
            }

            if (_cur == null) _cur = new OFBar { OpenPx = Open[0] };
            _cur.ClosePx = Close[0];

            Values[0][0] = _cvd + _cur.Delta;
            Values[1][0] = 0;

            if (ColorBars)
            {
                // Solo colorear el borde/outline de la vela, no el relleno
                // para no tapar las celdas del footprint
                if (_cur.Delta > 0)
                {
                    BarBrush        = System.Windows.Media.Brushes.Transparent;
                    CandleOutlineBrush = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromArgb(255, 34, 139, 34));
                }
                else if (_cur.Delta < 0)
                {
                    BarBrush        = System.Windows.Media.Brushes.Transparent;
                    CandleOutlineBrush = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromArgb(255, 178, 34, 34));
                }
            }
        }

        private void FinalizeBar(OFBar b)
        {
            if (b.Levels.Count < 2) return;
            double ratio = ImbalPct / 100.0;
            var    keys  = b.Levels.Keys.ToList();

            for (int i = 0; i < keys.Count - 1; i++)
            {
                var lo = b.Levels[keys[i]];
                var hi = b.Levels[keys[i + 1]];

                if (hi.Bid > 0 && lo.Ask > 0 && lo.Ask / hi.Bid >= ratio)
                    b.ImbalAsk.Add(keys[i]);

                if (lo.Ask > 0 && hi.Bid > 0 && hi.Bid / lo.Ask >= ratio)
                    b.ImbalBid.Add(keys[i + 1]);
            }

            bool bullish = b.ClosePx >= b.OpenPx;
            b.Divergence = bullish != (b.Delta >= 0);
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════
        #region Renderizado DirectX
        // ═══════════════════════════════════════════════════════════════

        public override void OnRenderTargetChanged()
        { DisposeDX(); _dxOk = false; }

        protected override void OnRender(ChartControl cc, ChartScale cs)
        {
            base.OnRender(cc, cs);
            if (RenderTarget == null) return;
            if (!_dxOk) { InitDX(); _dxOk = true; }

            for (int i = ChartBars.FromIndex; i <= ChartBars.ToIndex; i++)
            {
                OFBar b = (i == CurrentBar && _cur != null) ? _cur
                        : (_hist.ContainsKey(i) ? _hist[i] : null);
                if (b == null || b.Levels.Count == 0) continue;

                float cx = cc.GetXByBarIndex(ChartBars, i);
                DrawBar(cs, b, cx);
            }
        }

        private void DrawBar(ChartScale cs, OFBar b, float cx)
        {
            float cw = (float)CellW;

            double rowSize = TickSize * Math.Max(1, TicksPerRow);
            float tickPx   = (float)cs.GetPixelsForDistance(rowSize);

            // Altura de celda: espacio real de la vela dividido entre filas
            // Garantiza que las celdas llenen la vela visualmente
            int   numRows  = b.Levels.Count > 0 ? b.Levels.Count : 1;
            float barTopY  = b.Levels.Count > 0 ? (float)cs.GetYByValue(b.Levels.Keys.Max()) : 0f;
            float barBotY  = b.Levels.Count > 0 ? (float)cs.GetYByValue(b.Levels.Keys.Min()) : 0f;
            float barPxH   = Math.Abs(barBotY - barTopY) + tickPx; // altura total en px
            float ch       = Math.Max((float)CellH, barPxH / numRows);

            // Fuente que quepa en la celda
            float fSize = Math.Max(7f, Math.Min((float)FontPx, ch * 0.72f));
            if (Math.Abs(fSize - _tfSize) > 0.5f)
            {
                _tfNormal?.Dispose(); _tfBold?.Dispose();
                _tfNormal = new TextFormat(Core.Globals.DirectWriteFactory,
                    "Consolas", FontWeight.Normal, FontStyle.Normal, FontStretch.Normal, fSize)
                    { TextAlignment = TextAlignment.Center, ParagraphAlignment = ParagraphAlignment.Center };
                _tfBold = new TextFormat(Core.Globals.DirectWriteFactory,
                    "Consolas", FontWeight.Bold, FontStyle.Normal, FontStretch.Normal, fSize)
                    { TextAlignment = TextAlignment.Center, ParagraphAlignment = ParagraphAlignment.Center };
                _tfSize = fSize;
            }

            float left = cx - cw * 0.5f;

            foreach (var kv in b.Levels)
            {
                double price = kv.Key;
                var    lv    = kv.Value;

                // Posición Y centrada en el precio
                float py  = (float)cs.GetYByValue(price);
                float top = py - ch * 0.5f;

                var rect = new RectangleF(left, top, cw, ch);

                bool isPOC  = Math.Abs(price - b.POC) < rowSize * 0.5;
                bool isIAsk = b.ImbalAsk.Contains(price);
                bool isIBid = b.ImbalBid.Contains(price);

                // ── Fondo blanco base siempre ─────────────────────────
                RenderTarget.FillRectangle(rect, _bWhiteBg);

                // ── Color encima con intensidad proporcional al volumen
                float volRatio = b.MaxVol > 0
                    ? (float)Math.Min(lv.Total / b.MaxVol, 1.0)
                    : 0f;
                float alpha = 0.05f + volRatio * 0.80f;

                SharpDX.Direct2D1.Brush bg;
                if      (isPOC)  { bg = _bPocBg;  alpha = 0.85f; }
                else if (isIAsk)   bg = _bAskBg;
                else if (isIBid)   bg = _bBidBg;
                else               bg = _bVolBg;

                var prev = bg.Opacity;
                bg.Opacity = alpha;
                RenderTarget.FillRectangle(rect, bg);
                bg.Opacity = prev;

                // ── Borde ─────────────────────────────────────────────
                RenderTarget.DrawRectangle(rect, _bBorder, 0.8f);

                // ── Texto "BID x ASK" ─────────────────────────────────
                if (_tfNormal == null) continue;

                string bidStr = FmtVol(lv.Bid);
                string askStr = FmtVol(lv.Ask);
                string full   = bidStr + " x " + askStr;

                // Texto: negro sobre celdas claras, blanco sobre celdas con color intenso
                SharpDX.Direct2D1.Brush txtBrush = (alpha > 0.45f || isPOC)
                    ? _bWhite : _bDarkTxt;

                // En imbalances usar bold, sino normal
                var tf = (isIAsk || isIBid || isPOC) ? _tfBold : _tfNormal;
                DrawText(full, rect, tf, txtBrush);
            }

            // ── Delta debajo de la vela ────────────────────────────────
            if (ShowDelta && _tfBold != null && b.Levels.Count > 0)
            {
                float loY    = (float)cs.GetYByValue(b.Levels.Keys.Min()) + ch * 0.5f + 3f;
                var   dRect  = new RectangleF(left, loY, cw, ch);
                string dtxt  = (b.Delta >= 0 ? "▲" : "▼") + FmtVol(Math.Abs(b.Delta));
                var    dBrush = b.Delta >= 0 ? _bDeltaPos : _bDeltaNeg;

                RenderTarget.FillRectangle(dRect, _bNormalBg);
                DrawText(dtxt, dRect, _tfBold, dBrush);
            }

            // ── Triángulo divergencia ──────────────────────────────────
            if (b.Divergence && b.Levels.Count > 0)
            {
                float hiY = (float)cs.GetYByValue(b.Levels.Keys.Max()) - ch * 0.5f - 14f;
                DrawTriangle(cx, hiY, 8f);
            }
        }

        // ── Texto centrado en rect ─────────────────────────────────────
        private void DrawText(string s, RectangleF r, TextFormat tf,
                              SharpDX.Direct2D1.Brush brush)
        {
            using (var lay = new TextLayout(
                       Core.Globals.DirectWriteFactory, s, tf, r.Width, r.Height))
            {
                RenderTarget.DrawTextLayout(
                    new Vector2(r.X + (r.Width  - lay.Metrics.Width)  * 0.5f,
                                r.Y + (r.Height - lay.Metrics.Height) * 0.5f),
                    lay, brush);
            }
        }

        // ── Triángulo ─────────────────────────────────────────────────
        private void DrawTriangle(float cx, float ty, float sz)
        {
            using (var geo  = new SharpDX.Direct2D1.PathGeometry(Core.Globals.D2DFactory))
            using (var sink = geo.Open())
            {
                sink.BeginFigure(new Vector2(cx,     ty),           FigureBegin.Filled);
                sink.AddLine(   new Vector2(cx - sz, ty + sz * 1.7f));
                sink.AddLine(   new Vector2(cx + sz, ty + sz * 1.7f));
                sink.EndFigure(FigureEnd.Closed);
                sink.Close();
                RenderTarget.FillGeometry(geo, _bDiv);
            }
        }

        // ── Brushes ─────────────────────────────────────────────────
        private void InitDX()
        {
            _bWhiteBg  = Brush(255, 255, 255, 255);  // blanco sólido
            _bNormalBg = Brush(255, 255, 255, 255);  // mismo blanco para delta bg
            _bVolBg    = Brush( 80, 120, 200, 255);  // azul — modula con Opacity
            _bAskBg    = Brush( 34, 139,  34, 255);  // verde
            _bBidBg    = Brush(200,  40,  40, 255);  // rojo
            _bPocBg    = Brush(200, 170,   0, 255);  // amarillo
            _bBorder   = Brush(180, 180, 180, 200);  // gris claro
            _bWhite    = Brush(255, 255, 255, 255);  // texto blanco (sobre colores intensos)
            _bDarkTxt  = Brush( 20,  20,  20, 255);  // texto negro (sobre fondo claro)
            _bDeltaPos = Brush( 34, 139,  34, 255);
            _bDeltaNeg = Brush(200,  40,  40, 255);
            _bDiv      = Brush(210,   0, 210, 230);
        }

        private SharpDX.Direct2D1.Brush Brush(byte r, byte g, byte b, byte a)
            => new SharpDX.Direct2D1.SolidColorBrush(RenderTarget,
               new SharpDX.Color4(r / 255f, g / 255f, b / 255f, a / 255f));

        private void DisposeDX()
        {
            _tfNormal?.Dispose(); _tfNormal = null;
            _tfBold?.Dispose();   _tfBold   = null;
            _tfSize = -1f;

            _bWhiteBg?.Dispose();  _bWhiteBg  = null;
            _bNormalBg?.Dispose(); _bNormalBg = null;
            _bVolBg?.Dispose();    _bVolBg    = null;
            _bAskBg?.Dispose();    _bAskBg    = null;
            _bBidBg?.Dispose();    _bBidBg    = null;
            _bPocBg?.Dispose();    _bPocBg    = null;
            _bBorder?.Dispose();   _bBorder   = null;
            _bWhite?.Dispose();    _bWhite    = null;
            _bDarkTxt?.Dispose();  _bDarkTxt  = null;
            _bDeltaPos?.Dispose(); _bDeltaPos = null;
            _bDeltaNeg?.Dispose(); _bDeltaNeg = null;
            _bDiv?.Dispose();      _bDiv      = null;
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════
        #region Utilidades
        // ═══════════════════════════════════════════════════════════════

        private static string FmtVol(double v)
        {
            if (v >= 1_000_000) return (v / 1_000_000.0).ToString("0.0") + "M";
            if (v >= 1_000)     return (v / 1_000.0).ToString("0.#")     + "K";
            return v.ToString("0");
        }

        #endregion
    }
}
