using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace TUM.CMS.VplControl.Core
{
    public class Theme
    {
        private readonly VplControl hostCanvas;

        public Theme(VplControl hostCanvas)
        {
            this.hostCanvas = hostCanvas;
        }

        #region General

        private Color backgroundColor;

        [Category("General style")]
        [DisplayName(@"Background color")]
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                backgroundColor = value;
                Application.Current.Resources["BackgroundBrush"] = new SolidColorBrush(backgroundColor);
            }
        }

        private Color gridColor;

        [Category("General style")]
        [DisplayName(@"Grid color")]
        [Browsable(false)]
        public Color GridColor
        {
            get { return gridColor; }
            set
            {
                gridColor = value;
                Application.Current.Resources["GridBrush"] = new SolidColorBrush(gridColor);
            }
        }

        private FontFamily fontFamily;

        [Category("General style")]
        [DisplayName(@"Font family")]
        public FontFamily FontFamily
        {
            get { return fontFamily; }
            set
            {
                fontFamily = value;

                TextElement.SetFontFamily(hostCanvas, fontFamily);
            }
        }

        private Color foregroundColor;

        [Category("General style")]
        [DisplayName(@"Foreground color")]
        public Color ForegroundColor
        {
            get { return foregroundColor; }
            set
            {
                foregroundColor = value;
                TextElement.SetForeground(hostCanvas, new SolidColorBrush(foregroundColor));
            }
        }

        private Color highlightingColor;

        [Category("General style")]
        [DisplayName(@"Highlighting color")]
        public Color HighlightingColor
        {
            get { return highlightingColor; }
            set
            {
                highlightingColor = value;
                Application.Current.Resources["BrushHighlighting"] = new SolidColorBrush(highlightingColor);
            }
        }

        #endregion

        #region Connector

        private Color connectorColor;

        [Category("Connector")]
        [DisplayName(@"Color")]
        public Color ConnectorColor
        {
            get { return connectorColor; }
            set
            {
                connectorColor = value;
                Application.Current.Resources["ConnectorBrush"] = new SolidColorBrush(connectorColor);
            }
        }


        private double connectorThickness;

        [Category("Connector")]
        [DisplayName(@"Thickness")]
        public double ConnectorThickness
        {
            get { return connectorThickness; }
            set
            {
                connectorThickness = value;
                Application.Current.Resources["ConnectorThickness"] = connectorThickness;
            }
        }

        private Color connEllipseFillColor;

        [Category("Connector")]
        [DisplayName(@"Ellipse fill color")]
        public Color ConnEllipseFillColor
        {
            get { return connEllipseFillColor; }
            set
            {
                connEllipseFillColor = value;
                Application.Current.Resources["ConnEllipseFillBrush"] = new SolidColorBrush(connEllipseFillColor);
            }
        }


        private double connEllipseSize;

        [Category("Connector")]
        [DisplayName(@"Ellipse size")]
        public double ConnEllipseSize
        {
            get { return connEllipseSize; }
            set
            {
                connEllipseSize = value;
                Application.Current.Resources["ConnEllipseSize"] = connEllipseSize;
            }
        }

        #endregion

        #region Port

        private Color portFillColor;

        [Category("Port")]
        [DisplayName(@"Fill Color")]
        public Color PortFillColor
        {
            get { return portFillColor; }
            set
            {
                portFillColor = value;
                Application.Current.Resources["PortFillBrush"] = new SolidColorBrush(portFillColor);
            }
        }

        private Color portStrokeColor;

        [Category("Port")]
        [DisplayName(@"Stroke Color")]
        public Color PortStrokeColor
        {
            get { return portStrokeColor; }
            set
            {
                portStrokeColor = value;
                Application.Current.Resources["PortStrokeBrush"] = new SolidColorBrush(portStrokeColor);
            }
        }


        private double portSize;

        [Category("Port")]
        [DisplayName(@"Size")]
        public double PortSize
        {
            get { return portSize; }
            set
            {
                portSize = value;
                Application.Current.Resources["PortSize"] = portSize;
            }
        }

        private double portStrokeThickness;

        [Category("Port")]
        [DisplayName(@"Stroke thickness")]
        public double PortStrokeThickness
        {
            get { return portStrokeThickness; }
            set
            {
                portStrokeThickness = value;
                Application.Current.Resources["PortStrokeThickness"] = portStrokeThickness;
            }
        }

        #endregion

        #region Button

        private Color buttonBorderColor;

        [Category("Button style")]
        [DisplayName(@"Border color")]
        public Color ButtonBorderColor
        {
            get { return buttonBorderColor; }
            set
            {
                buttonBorderColor = value;
                Application.Current.Resources["ButtonBorderBrush"] = new SolidColorBrush(buttonBorderColor);
            }
        }

        private Color buttonFillColor;

        [Category("Button style")]
        [DisplayName(@"Fill color")]
        public Color ButtonFillColor
        {
            get { return buttonFillColor; }
            set
            {
                buttonFillColor = value;
                Application.Current.Resources["ButtonFillBrush"] = new SolidColorBrush(buttonFillColor);
            }
        }

        #endregion

        #region Tooltip

        private Color tooltipBorderColor;

        [Category("Tooltip")]
        [DisplayName(@"Border color")]
        public Color TooltipBorderColor
        {
            get { return tooltipBorderColor; }
            set
            {
                tooltipBorderColor = value;
                Application.Current.Resources["TooltipBorderBrush"] = new SolidColorBrush(tooltipBorderColor);
            }
        }

        private Color tooltipBackgroundColor;

        [Category("Tooltip")]
        [DisplayName(@"Background color")]
        public Color TooltipBackgroundColor
        {
            get { return tooltipBackgroundColor; }
            set
            {
                tooltipBackgroundColor = value;
                Application.Current.Resources["TooltipBackgroundBrush"] = new SolidColorBrush(tooltipBackgroundColor);
            }
        }

        #endregion

        #region Line

        private Color lineColor;

        [Category("Line")]
        [DisplayName(@"Color")]
        public Color LineColor
        {
            get { return lineColor; }
            set
            {
                lineColor = value;
                Application.Current.Resources["LineStrokeBrush"] = new SolidColorBrush(lineColor);
            }
        }


        private double lineThickness;

        [Category("Line")]
        [DisplayName(@"Thickness")]
        public double LineThickness
        {
            get { return lineThickness; }
            set
            {
                lineThickness = value;
                Application.Current.Resources["LineStrokeThickness"] = lineThickness;
            }
        }

        #endregion

        #region Node

        private Color nodeBorderColor;

        [Category("Node style")]
        [DisplayName(@"Border color")]
        public Color NodeBorderColor
        {
            get { return nodeBorderColor; }
            set
            {
                nodeBorderColor = value;
                Application.Current.Resources["NodeBorderBrush"] = new SolidColorBrush(nodeBorderColor);
            }
        }

        private Thickness nodeBorderThickness;

        [Category("Node style")]
        [DisplayName(@"Border thickness")]
        public Thickness NodeBorderThickness
        {
            get { return nodeBorderThickness; }
            set
            {
                nodeBorderThickness = value;
                Application.Current.Resources["NodeBorderThickness"] = nodeBorderThickness;
            }
        }

        private double nodeBorderCornerRadius;

        [Category("Node style")]
        [DisplayName(@"Border corner radius")]
        public double NodeBorderCornerRadius
        {
            get { return nodeBorderCornerRadius; }
            set
            {
                nodeBorderCornerRadius = value;
                Application.Current.Resources["NodeBorderCornerRadius"] = nodeBorderCornerRadius;
            }
        }


        private Color nodeBackgroundColor;

        [Category("Node style")]
        [DisplayName(@"Background color")]
        public Color NodeBackgroundColor
        {
            get { return nodeBackgroundColor; }
            set
            {
                nodeBackgroundColor = value;
                Application.Current.Resources["NodeBackgroundBrush"] = new SolidColorBrush(nodeBackgroundColor);
            }
        }

        private Color nodeBorderColorOnMouseOver;

        [Category("Node style")]
        [DisplayName(@"Border MouseOver color")]
        public Color NodeBorderColorOnMouseOver
        {
            get { return nodeBorderColorOnMouseOver; }
            set
            {
                nodeBorderColorOnMouseOver = value;
                Application.Current.Resources["NodeBorderBrushMouseOver"] =
                    new SolidColorBrush(nodeBorderColorOnMouseOver = value);
            }
        }


        private Color nodeBorderColorOnSelection;

        [Category("Node style")]
        [DisplayName(@"Border selection color")]
        public Color NodeBorderColorOnSelection
        {
            get { return nodeBorderColorOnSelection; }
            set
            {
                nodeBorderColorOnSelection = value;
                Application.Current.Resources["NodeBorderBrushSelection"] = new SolidColorBrush(nodeBorderColorOnSelection);
            }
        }

        #endregion
    }
}