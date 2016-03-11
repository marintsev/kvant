namespace ThreadMan
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
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
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.btnToggle = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnClean = new System.Windows.Forms.Button();
            this.btnTerminate = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(360, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(108, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Создать";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnToggle
            // 
            this.btnToggle.Location = new System.Drawing.Point(360, 41);
            this.btnToggle.Name = "btnToggle";
            this.btnToggle.Size = new System.Drawing.Size(108, 23);
            this.btnToggle.TabIndex = 1;
            this.btnToggle.Text = "Приостановить";
            this.btnToggle.UseVisualStyleBackColor = true;
            this.btnToggle.Click += new System.EventHandler(this.btnToggle_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(360, 70);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(108, 23);
            this.btnAbort.TabIndex = 2;
            this.btnAbort.Text = "Прервать";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(342, 277);
            this.listBox1.TabIndex = 3;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            this.listBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDoubleClick);
            // 
            // btnClean
            // 
            this.btnClean.Location = new System.Drawing.Point(360, 100);
            this.btnClean.Name = "btnClean";
            this.btnClean.Size = new System.Drawing.Size(108, 23);
            this.btnClean.TabIndex = 4;
            this.btnClean.Text = "Очистить";
            this.btnClean.UseVisualStyleBackColor = true;
            this.btnClean.Click += new System.EventHandler(this.button4_Click);
            // 
            // btnTerminate
            // 
            this.btnTerminate.Location = new System.Drawing.Point(360, 130);
            this.btnTerminate.Name = "btnTerminate";
            this.btnTerminate.Size = new System.Drawing.Size(108, 23);
            this.btnTerminate.TabIndex = 5;
            this.btnTerminate.Text = "Остановить все";
            this.btnTerminate.UseVisualStyleBackColor = true;
            this.btnTerminate.Click += new System.EventHandler(this.btnTerminate_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 304);
            this.Controls.Add(this.btnTerminate);
            this.Controls.Add(this.btnClean);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnToggle);
            this.Controls.Add(this.button1);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Создание потоков с длительными вычислениями";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnToggle;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button btnClean;
        private System.Windows.Forms.Button btnTerminate;
        private System.Windows.Forms.Timer timer1;

    }
}

