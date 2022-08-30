namespace DogCatClassif
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.loadBtn = new System.Windows.Forms.Button();
            this.isCatBtn = new System.Windows.Forms.Button();
            this.isDogBtn = new System.Windows.Forms.Button();
            this.TrainBtn = new System.Windows.Forms.Button();
            this.predictBtn = new System.Windows.Forms.Button();
            this.resultTxt = new System.Windows.Forms.Label();
            this.chartVisual1 = new AI.Charts.Control.ChartVisual();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictureBox1.Location = new System.Drawing.Point(13, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(435, 393);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // loadBtn
            // 
            this.loadBtn.Location = new System.Drawing.Point(12, 412);
            this.loadBtn.Name = "loadBtn";
            this.loadBtn.Size = new System.Drawing.Size(148, 54);
            this.loadBtn.TabIndex = 1;
            this.loadBtn.Text = "Путь до папки";
            this.loadBtn.UseVisualStyleBackColor = true;
            this.loadBtn.Click += new System.EventHandler(this.loadBtn_Click);
            // 
            // isCatBtn
            // 
            this.isCatBtn.Location = new System.Drawing.Point(167, 413);
            this.isCatBtn.Name = "isCatBtn";
            this.isCatBtn.Size = new System.Drawing.Size(75, 23);
            this.isCatBtn.TabIndex = 2;
            this.isCatBtn.Text = "Кот";
            this.isCatBtn.UseVisualStyleBackColor = true;
            this.isCatBtn.Click += new System.EventHandler(this.isCatBtn_Click);
            // 
            // isDogBtn
            // 
            this.isDogBtn.Location = new System.Drawing.Point(167, 442);
            this.isDogBtn.Name = "isDogBtn";
            this.isDogBtn.Size = new System.Drawing.Size(75, 23);
            this.isDogBtn.TabIndex = 2;
            this.isDogBtn.Text = "Пес";
            this.isDogBtn.UseVisualStyleBackColor = true;
            this.isDogBtn.Click += new System.EventHandler(this.isDogBtn_Click);
            // 
            // TrainBtn
            // 
            this.TrainBtn.Location = new System.Drawing.Point(248, 414);
            this.TrainBtn.Name = "TrainBtn";
            this.TrainBtn.Size = new System.Drawing.Size(97, 51);
            this.TrainBtn.TabIndex = 3;
            this.TrainBtn.Text = "Обучить";
            this.TrainBtn.UseVisualStyleBackColor = true;
            this.TrainBtn.Click += new System.EventHandler(this.TrainBtn_Click);
            // 
            // predictBtn
            // 
            this.predictBtn.Location = new System.Drawing.Point(351, 412);
            this.predictBtn.Name = "predictBtn";
            this.predictBtn.Size = new System.Drawing.Size(97, 51);
            this.predictBtn.TabIndex = 3;
            this.predictBtn.Text = "Распознать";
            this.predictBtn.UseVisualStyleBackColor = true;
            this.predictBtn.Click += new System.EventHandler(this.predictBtn_Click);
            // 
            // resultTxt
            // 
            this.resultTxt.AutoSize = true;
            this.resultTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.resultTxt.Location = new System.Drawing.Point(12, 481);
            this.resultTxt.Name = "resultTxt";
            this.resultTxt.Size = new System.Drawing.Size(112, 20);
            this.resultTxt.TabIndex = 4;
            this.resultTxt.Text = "Кот или пес? ";
            // 
            // chartVisual1
            // 
            this.chartVisual1.AutoScroll = true;
            this.chartVisual1.BackColor = System.Drawing.Color.White;
            this.chartVisual1.ChartName = "Вероятности классов";
            this.chartVisual1.ForeColor = System.Drawing.Color.Black;
            this.chartVisual1.IsContextMenu = true;
            this.chartVisual1.IsLogScale = false;
            this.chartVisual1.IsMoove = true;
            this.chartVisual1.IsScale = true;
            this.chartVisual1.IsShowXY = true;
            this.chartVisual1.LabelX = "Ось Х";
            this.chartVisual1.LabelY = "Ось Y";
            this.chartVisual1.Location = new System.Drawing.Point(455, 13);
            this.chartVisual1.Name = "chartVisual1";
            this.chartVisual1.Size = new System.Drawing.Size(520, 488);
            this.chartVisual1.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(979, 510);
            this.Controls.Add(this.chartVisual1);
            this.Controls.Add(this.resultTxt);
            this.Controls.Add(this.predictBtn);
            this.Controls.Add(this.TrainBtn);
            this.Controls.Add(this.isDogBtn);
            this.Controls.Add(this.isCatBtn);
            this.Controls.Add(this.loadBtn);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.Text = "DogCat";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button loadBtn;
        private System.Windows.Forms.Button isCatBtn;
        private System.Windows.Forms.Button isDogBtn;
        private System.Windows.Forms.Button TrainBtn;
        private System.Windows.Forms.Button predictBtn;
        private System.Windows.Forms.Label resultTxt;
        private AI.Charts.Control.ChartVisual chartVisual1;
    }
}

