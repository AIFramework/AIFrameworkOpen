namespace SpectrumAnalyzer
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
            this.button1 = new System.Windows.Forms.Button();
            this.spectrumWelchAnalyzer1 = new AI.Charts.Control.SpectrumWelchAnalyzer();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(691, 36);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // spectrumWelchAnalyzer1
            // 
            this.spectrumWelchAnalyzer1.FFTBlock = 1024;
            this.spectrumWelchAnalyzer1.FreqOffset = 0D;
            this.spectrumWelchAnalyzer1.Location = new System.Drawing.Point(53, 13);
            this.spectrumWelchAnalyzer1.Name = "spectrumWelchAnalyzer1";
            this.spectrumWelchAnalyzer1.Size = new System.Drawing.Size(632, 379);
            this.spectrumWelchAnalyzer1.SR = 80000;
            this.spectrumWelchAnalyzer1.TabIndex = 2;
            this.spectrumWelchAnalyzer1.WelchPSDTypeData = AI.DSP.Analyse.WelchPSDType.Db;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.spectrumWelchAnalyzer1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private AI.Charts.Control.SpectrumWelchAnalyzer spectrumWelchAnalyzer1;
    }
}

