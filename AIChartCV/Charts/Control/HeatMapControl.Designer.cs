namespace AI.Charts.Control
{
    partial class HeatMapControl
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

    #region Код, автоматически созданный конструктором компонентов

    /// <summary> 
    /// Требуемый метод для поддержки конструктора — не изменяйте 
    /// содержимое этого метода с помощью редактора кода.
    /// </summary>
    private void InitializeComponent()
    {
        this.mainPict = new System.Windows.Forms.PictureBox();
        this.gradient = new System.Windows.Forms.PictureBox();
        this.maxLabel = new System.Windows.Forms.Label();
        this.minLabel = new System.Windows.Forms.Label();
        this.meanLabel = new System.Windows.Forms.Label();
        this.label1 = new System.Windows.Forms.Label();
        this.xValue = new System.Windows.Forms.Label();
        this.yValue = new System.Windows.Forms.Label();
        this.xyValue = new System.Windows.Forms.Label();
        this.q75 = new System.Windows.Forms.Label();
        this.q25 = new System.Windows.Forms.Label();
        ((System.ComponentModel.ISupportInitialize)(this.mainPict)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.gradient)).BeginInit();
        this.SuspendLayout();
        // 
        // mainPict
        // 
        this.mainPict.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
        | System.Windows.Forms.AnchorStyles.Left)
        | System.Windows.Forms.AnchorStyles.Right)));
        this.mainPict.BackColor = System.Drawing.SystemColors.ButtonHighlight;
        this.mainPict.Location = new System.Drawing.Point(3, 19);
        this.mainPict.Name = "mainPict";
        this.mainPict.Size = new System.Drawing.Size(200, 200);
        this.mainPict.TabIndex = 0;
        this.mainPict.TabStop = false;
        this.mainPict.SizeChanged += new System.EventHandler(this.MainPict_SizeChanged);
        // 
        // gradient
        // 
        this.gradient.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
        | System.Windows.Forms.AnchorStyles.Right)));
        this.gradient.BackColor = System.Drawing.SystemColors.ButtonHighlight;
        this.gradient.Location = new System.Drawing.Point(210, 19);
        this.gradient.Name = "gradient";
        this.gradient.Size = new System.Drawing.Size(20, 200);
        this.gradient.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
        this.gradient.TabIndex = 1;
        this.gradient.TabStop = false;
        this.gradient.SizeChanged += new System.EventHandler(this.Gradient_SizeChanged);
        // 
        // maxLabel
        // 
        this.maxLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.maxLabel.AutoSize = true;
        this.maxLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.maxLabel.Location = new System.Drawing.Point(236, 19);
        this.maxLabel.Name = "maxLabel";
        this.maxLabel.Size = new System.Drawing.Size(39, 15);
        this.maxLabel.TabIndex = 2;
        this.maxLabel.Text = "100%";
        // 
        // minLabel
        // 
        this.minLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.minLabel.AutoSize = true;
        this.minLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.minLabel.Location = new System.Drawing.Point(236, 201);
        this.minLabel.Name = "minLabel";
        this.minLabel.Size = new System.Drawing.Size(25, 15);
        this.minLabel.TabIndex = 3;
        this.minLabel.Text = "0%";
        // 
        // meanLabel
        // 
        this.meanLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
        this.meanLabel.AutoSize = true;
        this.meanLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.meanLabel.Location = new System.Drawing.Point(236, 108);
        this.meanLabel.Name = "meanLabel";
        this.meanLabel.Size = new System.Drawing.Size(32, 15);
        this.meanLabel.TabIndex = 4;
        this.meanLabel.Text = "50%";
        // 
        // label1
        // 
        this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(3, 222);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(22, 13);
        this.label1.TabIndex = 5;
        this.label1.Text = "0,0";
        // 
        // xValue
        // 
        this.xValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.xValue.AutoSize = true;
        this.xValue.Location = new System.Drawing.Point(171, 222);
        this.xValue.Name = "xValue";
        this.xValue.Size = new System.Drawing.Size(22, 13);
        this.xValue.TabIndex = 6;
        this.xValue.Text = "0,0";
        // 
        // yValue
        // 
        this.yValue.AutoSize = true;
        this.yValue.Location = new System.Drawing.Point(3, 3);
        this.yValue.Name = "yValue";
        this.yValue.Size = new System.Drawing.Size(22, 13);
        this.yValue.TabIndex = 7;
        this.yValue.Text = "0,0";
        // 
        // xyValue
        // 
        this.xyValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.xyValue.AutoSize = true;
        this.xyValue.Location = new System.Drawing.Point(171, 3);
        this.xyValue.Name = "xyValue";
        this.xyValue.Size = new System.Drawing.Size(22, 13);
        this.xyValue.TabIndex = 8;
        this.xyValue.Text = "0,0";
        // 
        // q75
        // 
        this.q75.Anchor = System.Windows.Forms.AnchorStyles.Right;
        this.q75.AutoSize = true;
        this.q75.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.q75.Location = new System.Drawing.Point(236, 60);
        this.q75.Name = "q75";
        this.q75.Size = new System.Drawing.Size(32, 15);
        this.q75.TabIndex = 9;
        this.q75.Text = "75%";
        // 
        // q25
        // 
        this.q25.Anchor = System.Windows.Forms.AnchorStyles.Right;
        this.q25.AutoSize = true;
        this.q25.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.q25.Location = new System.Drawing.Point(236, 155);
        this.q25.Name = "q25";
        this.q25.Size = new System.Drawing.Size(32, 15);
        this.q25.TabIndex = 10;
        this.q25.Text = "25%";
        // 
        // HeatMap
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add(this.q25);
        this.Controls.Add(this.q75);
        this.Controls.Add(this.xyValue);
        this.Controls.Add(this.yValue);
        this.Controls.Add(this.xValue);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.meanLabel);
        this.Controls.Add(this.minLabel);
        this.Controls.Add(this.maxLabel);
        this.Controls.Add(this.gradient);
        this.Controls.Add(this.mainPict);
        this.Name = "HeatMap";
        this.Size = new System.Drawing.Size(282, 239);
        this.Load += new System.EventHandler(this.HeatMap_Load);
        this.SizeChanged += new System.EventHandler(this.HeatMap_SizeChanged);
        ((System.ComponentModel.ISupportInitialize)(this.mainPict)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.gradient)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox mainPict;
    private System.Windows.Forms.PictureBox gradient;
    private System.Windows.Forms.Label maxLabel;
    private System.Windows.Forms.Label minLabel;
    private System.Windows.Forms.Label meanLabel;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label xValue;
    private System.Windows.Forms.Label yValue;
    private System.Windows.Forms.Label xyValue;
    private System.Windows.Forms.Label q75;
    private System.Windows.Forms.Label q25;
}
}
