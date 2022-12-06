namespace WorldModel
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.heatMapControl1 = new AI.Charts.Control.HeatMapControl();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.chartVisual1 = new AI.Charts.Control.ChartVisual();
            this.heatMapControl2 = new AI.Charts.Control.HeatMapControl();
            this.chartVisual2 = new AI.Charts.Control.ChartVisual();
            this.SuspendLayout();
            // 
            // heatMapControl1
            // 
            this.heatMapControl1.Location = new System.Drawing.Point(4, 7);
            this.heatMapControl1.Name = "heatMapControl1";
            this.heatMapControl1.Size = new System.Drawing.Size(578, 429);
            this.heatMapControl1.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 300;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // chartVisual1
            // 
            this.chartVisual1.AutoScroll = true;
            this.chartVisual1.BackColor = System.Drawing.Color.White;
            this.chartVisual1.ChartName = "График";
            this.chartVisual1.ForeColor = System.Drawing.Color.Black;
            this.chartVisual1.IsContextMenu = true;
            this.chartVisual1.IsLogScale = false;
            this.chartVisual1.IsMoove = true;
            this.chartVisual1.IsScale = true;
            this.chartVisual1.IsShowXY = true;
            this.chartVisual1.LabelX = "Ось Х";
            this.chartVisual1.LabelY = "Ось Y";
            this.chartVisual1.Location = new System.Drawing.Point(588, 7);
            this.chartVisual1.Name = "chartVisual1";
            this.chartVisual1.Size = new System.Drawing.Size(558, 429);
            this.chartVisual1.TabIndex = 1;
            // 
            // heatMapControl2
            // 
            this.heatMapControl2.Location = new System.Drawing.Point(4, 468);
            this.heatMapControl2.Name = "heatMapControl2";
            this.heatMapControl2.Size = new System.Drawing.Size(578, 429);
            this.heatMapControl2.TabIndex = 0;
            // 
            // chartVisual2
            // 
            this.chartVisual2.AutoScroll = true;
            this.chartVisual2.BackColor = System.Drawing.Color.White;
            this.chartVisual2.ChartName = "График";
            this.chartVisual2.ForeColor = System.Drawing.Color.Black;
            this.chartVisual2.IsContextMenu = true;
            this.chartVisual2.IsLogScale = false;
            this.chartVisual2.IsMoove = true;
            this.chartVisual2.IsScale = true;
            this.chartVisual2.IsShowXY = true;
            this.chartVisual2.LabelX = "Ось Х";
            this.chartVisual2.LabelY = "Ось Y";
            this.chartVisual2.Location = new System.Drawing.Point(588, 468);
            this.chartVisual2.Name = "chartVisual2";
            this.chartVisual2.Size = new System.Drawing.Size(558, 429);
            this.chartVisual2.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1144, 937);
            this.Controls.Add(this.chartVisual2);
            this.Controls.Add(this.heatMapControl2);
            this.Controls.Add(this.chartVisual1);
            this.Controls.Add(this.heatMapControl1);
            this.Name = "Form1";
            this.Text = "World";
            this.ResumeLayout(false);

        }

        #endregion

        private AI.Charts.Control.HeatMapControl heatMapControl1;
        private System.Windows.Forms.Timer timer1;
        private AI.Charts.Control.ChartVisual chartVisual1;
        private AI.Charts.Control.HeatMapControl heatMapControl2;
        private AI.Charts.Control.ChartVisual chartVisual2;
    }
}

