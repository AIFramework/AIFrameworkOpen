namespace RLTest
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
            this.chartVisual1 = new AI.Charts.Control.ChartVisual();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chartVisual1
            // 
            this.chartVisual1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chartVisual1.AutoScroll = true;
            this.chartVisual1.BackColor = System.Drawing.Color.White;
            this.chartVisual1.ChartName = "Нормированный ревард";
            this.chartVisual1.ForeColor = System.Drawing.Color.Black;
            this.chartVisual1.IsContextMenu = true;
            this.chartVisual1.IsLogScale = false;
            this.chartVisual1.IsMoove = true;
            this.chartVisual1.IsScale = true;
            this.chartVisual1.IsShowXY = true;
            this.chartVisual1.LabelX = "Число партий";
            this.chartVisual1.LabelY = "Значение среднего реварда";
            this.chartVisual1.Location = new System.Drawing.Point(-2, 0);
            this.chartVisual1.Name = "chartVisual1";
            this.chartVisual1.Size = new System.Drawing.Size(1049, 454);
            this.chartVisual1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(-2, 428);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(172, 26);
            this.button1.TabIndex = 2;
            this.button1.Text = "Новая серия игр";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1041, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chartVisual1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.Text = "Обучение с подкреплением";
            this.ResumeLayout(false);

        }

        #endregion

        private AI.Charts.Control.ChartVisual chartVisual1;
        private System.Windows.Forms.Button button1;
    }
}

