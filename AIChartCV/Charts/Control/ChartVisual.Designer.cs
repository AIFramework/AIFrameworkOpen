namespace AI.Charts.Control
{
    partial class ChartVisual
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem сохранитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem отправитьИзображениеВБуферОбменаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem выборФонаToolStripMenuItem;
        private System.Windows.Forms.Label labelXY;
        private System.Windows.Forms.ToolStripMenuItem масштабToolStripMenuItem;

        /// <summary>
        /// Disposes resources used by the control.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.сохранитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.отправитьИзображениеВБуферОбменаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выборФонаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newWindowOutp = new System.Windows.Forms.ToolStripMenuItem();
            this.преобразованияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.спектрToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.гистограммаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.diff = new System.Windows.Forms.ToolStripMenuItem();
            this.integ = new System.Windows.Forms.ToolStripMenuItem();
            this.масштабToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labelXY = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.chart1.BackImageAlignment = System.Windows.Forms.DataVisualization.Charting.ChartImageAlignmentStyle.Center;
            this.chart1.BackImageTransparentColor = System.Drawing.SystemColors.ActiveBorder;
            this.chart1.BackImageWrapMode = System.Windows.Forms.DataVisualization.Charting.ChartImageWrapMode.Scaled;
            this.chart1.BackSecondaryColor = System.Drawing.Color.Black;
            this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            this.chart1.BorderSkin.BackImageTransparentColor = System.Drawing.SystemColors.Window;
            this.chart1.BorderSkin.BackImageWrapMode = System.Windows.Forms.DataVisualization.Charting.ChartImageWrapMode.Scaled;
            this.chart1.BorderSkin.BorderColor = System.Drawing.Color.Bisque;
            this.chart1.BorderSkin.PageColor = System.Drawing.Color.Red;
            chartArea1.Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
            chartArea1.Area3DStyle.WallWidth = 9;
            chartArea1.AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
            chartArea1.AxisX.LineWidth = 2;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.Silver;
            chartArea1.AxisX.ScaleBreakStyle.Enabled = true;
            chartArea1.AxisX.Title = "Ось Х";
            chartArea1.AxisX.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            chartArea1.AxisY.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
            chartArea1.AxisY.LineWidth = 2;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.Silver;
            chartArea1.AxisY.Title = "Ось Y";
            chartArea1.AxisY.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            chartArea1.BackColor = System.Drawing.Color.White;
            chartArea1.BorderColor = System.Drawing.Color.BlanchedAlmond;
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.ContextMenuStrip = this.contextMenu;
            this.chart1.Cursor = System.Windows.Forms.Cursors.Arrow;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(-2, 3);
            this.chart1.Name = "chart1";
            this.chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            this.chart1.Size = new System.Drawing.Size(447, 301);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart";
            title1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            title1.Name = "Title1";
            title1.Text = "График";
            this.chart1.Titles.Add(title1);
            this.chart1.Paint += new System.Windows.Forms.PaintEventHandler(this.chart1_Paint);
            this.chart1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseMove);
            this.chart1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseUp);
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.сохранитьToolStripMenuItem,
            this.отправитьИзображениеВБуферОбменаToolStripMenuItem,
            this.выборФонаToolStripMenuItem,
            this.newWindowOutp,
            this.преобразованияToolStripMenuItem,
            this.масштабToolStripMenuItem});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(302, 136);
            // 
            // сохранитьToolStripMenuItem
            // 
            this.сохранитьToolStripMenuItem.Name = "сохранитьToolStripMenuItem";
            this.сохранитьToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            this.сохранитьToolStripMenuItem.Text = "Сохранить";
            this.сохранитьToolStripMenuItem.Click += new System.EventHandler(this.сохранитьToolStripMenuItem_Click);
            // 
            // отправитьИзображениеВБуферОбменаToolStripMenuItem
            // 
            this.отправитьИзображениеВБуферОбменаToolStripMenuItem.Name = "отправитьИзображениеВБуферОбменаToolStripMenuItem";
            this.отправитьИзображениеВБуферОбменаToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            this.отправитьИзображениеВБуферОбменаToolStripMenuItem.Text = "Отправить изображение в буфер обмена";
            this.отправитьИзображениеВБуферОбменаToolStripMenuItem.Click += new System.EventHandler(this.отправитьИзображениеВБуферОбменаToolStripMenuItem_Click);
            // 
            // выборФонаToolStripMenuItem
            // 
            this.выборФонаToolStripMenuItem.Name = "выборФонаToolStripMenuItem";
            this.выборФонаToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            this.выборФонаToolStripMenuItem.Text = "Выбор фона";
            this.выборФонаToolStripMenuItem.Click += new System.EventHandler(this.выборФонаToolStripMenuItem_Click);
            // 
            // newWindowOutp
            // 
            this.newWindowOutp.Name = "newWindowOutp";
            this.newWindowOutp.Size = new System.Drawing.Size(301, 22);
            this.newWindowOutp.Text = "Вывести в отдельном окне";
            this.newWindowOutp.Click += new System.EventHandler(this.NewWindowOutp_Click);
            // 
            // преобразованияToolStripMenuItem
            // 
            this.преобразованияToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.спектрToolStripMenuItem,
            this.гистограммаToolStripMenuItem,
            this.diff,
            this.integ});
            this.преобразованияToolStripMenuItem.Name = "преобразованияToolStripMenuItem";
            this.преобразованияToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            this.преобразованияToolStripMenuItem.Text = "Преобразования";
            // 
            // спектрToolStripMenuItem
            // 
            this.спектрToolStripMenuItem.Name = "спектрToolStripMenuItem";
            this.спектрToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.спектрToolStripMenuItem.Text = "Спектр";
            this.спектрToolStripMenuItem.Click += new System.EventHandler(this.СпектрToolStripMenuItem_Click);
            // 
            // гистограммаToolStripMenuItem
            // 
            this.гистограммаToolStripMenuItem.Name = "гистограммаToolStripMenuItem";
            this.гистограммаToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.гистограммаToolStripMenuItem.Text = "Гистограмма";
            this.гистограммаToolStripMenuItem.Click += new System.EventHandler(this.ГистограммаToolStripMenuItem_Click);
            // 
            // diff
            // 
            this.diff.Name = "diff";
            this.diff.Size = new System.Drawing.Size(147, 22);
            this.diff.Text = "Производная";
            this.diff.Click += new System.EventHandler(this.Diff_Click);
            // 
            // integ
            // 
            this.integ.Name = "integ";
            this.integ.Size = new System.Drawing.Size(147, 22);
            this.integ.Text = "Интеграл";
            this.integ.Click += new System.EventHandler(this.Integ_Click);
            // 
            // масштабToolStripMenuItem
            // 
            this.масштабToolStripMenuItem.Name = "масштабToolStripMenuItem";
            this.масштабToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            this.масштабToolStripMenuItem.Text = "Масштаб по умолчанию";
            this.масштабToolStripMenuItem.Click += new System.EventHandler(this.масштабToolStripMenuItem_Click);
            // 
            // labelXY
            // 
            this.labelXY.AutoSize = true;
            this.labelXY.BackColor = System.Drawing.Color.Transparent;
            this.labelXY.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelXY.Location = new System.Drawing.Point(3, 0);
            this.labelXY.Name = "labelXY";
            this.labelXY.Size = new System.Drawing.Size(30, 13);
            this.labelXY.TabIndex = 1;
            this.labelXY.Text = "X: Y:";
            this.labelXY.Click += new System.EventHandler(this.LabelXY_Click);
            // 
            // ChartVisual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.labelXY);
            this.Controls.Add(this.chart1);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "ChartVisual";
            this.Size = new System.Drawing.Size(443, 302);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.ToolStripMenuItem преобразованияToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem спектрToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem гистограммаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newWindowOutp;
        private System.Windows.Forms.ToolStripMenuItem diff;
        private System.Windows.Forms.ToolStripMenuItem integ;
    }
}
