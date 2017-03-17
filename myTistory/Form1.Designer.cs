namespace myTistory
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btn_open = new System.Windows.Forms.Button();
            this.txb_path = new System.Windows.Forms.TextBox();
            this.btn_upload = new System.Windows.Forms.Button();
            this.btn_auth = new System.Windows.Forms.Button();
            this.axWebBrowser1 = new AxSHDocVw.AxWebBrowser();
            this.cb_blog = new System.Windows.Forms.ComboBox();
            this.btn_backup = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.axWebBrowser1)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_open
            // 
            this.btn_open.Location = new System.Drawing.Point(12, 12);
            this.btn_open.Name = "btn_open";
            this.btn_open.Size = new System.Drawing.Size(75, 23);
            this.btn_open.TabIndex = 0;
            this.btn_open.Text = "파일열기";
            this.btn_open.UseVisualStyleBackColor = true;
            this.btn_open.Click += new System.EventHandler(this.btn_open_Click);
            // 
            // txb_path
            // 
            this.txb_path.Location = new System.Drawing.Point(93, 12);
            this.txb_path.Name = "txb_path";
            this.txb_path.Size = new System.Drawing.Size(637, 21);
            this.txb_path.TabIndex = 1;
            // 
            // btn_upload
            // 
            this.btn_upload.Location = new System.Drawing.Point(12, 41);
            this.btn_upload.Name = "btn_upload";
            this.btn_upload.Size = new System.Drawing.Size(75, 23);
            this.btn_upload.TabIndex = 2;
            this.btn_upload.Text = "업로드";
            this.btn_upload.UseVisualStyleBackColor = true;
            this.btn_upload.Click += new System.EventHandler(this.btn_upload_Click);
            // 
            // btn_auth
            // 
            this.btn_auth.Location = new System.Drawing.Point(12, 70);
            this.btn_auth.Name = "btn_auth";
            this.btn_auth.Size = new System.Drawing.Size(75, 23);
            this.btn_auth.TabIndex = 3;
            this.btn_auth.Text = "인증";
            this.btn_auth.UseVisualStyleBackColor = true;
            this.btn_auth.Click += new System.EventHandler(this.btn_auth_Click);
            // 
            // axWebBrowser1
            // 
            this.axWebBrowser1.Enabled = true;
            this.axWebBrowser1.Location = new System.Drawing.Point(93, 70);
            this.axWebBrowser1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWebBrowser1.OcxState")));
            this.axWebBrowser1.Size = new System.Drawing.Size(637, 265);
            this.axWebBrowser1.TabIndex = 4;
            this.axWebBrowser1.DocumentComplete += new AxSHDocVw.DWebBrowserEvents2_DocumentCompleteEventHandler(this.axWebBrowser1_DocumentComplete);
            // 
            // cb_blog
            // 
            this.cb_blog.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_blog.FormattingEnabled = true;
            this.cb_blog.Location = new System.Drawing.Point(93, 39);
            this.cb_blog.Name = "cb_blog";
            this.cb_blog.Size = new System.Drawing.Size(121, 20);
            this.cb_blog.TabIndex = 5;
            // 
            // btn_backup
            // 
            this.btn_backup.Location = new System.Drawing.Point(12, 99);
            this.btn_backup.Name = "btn_backup";
            this.btn_backup.Size = new System.Drawing.Size(75, 23);
            this.btn_backup.TabIndex = 6;
            this.btn_backup.Text = "글백업";
            this.btn_backup.UseVisualStyleBackColor = true;
            this.btn_backup.Click += new System.EventHandler(this.btn_backup_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 347);
            this.Controls.Add(this.btn_backup);
            this.Controls.Add(this.cb_blog);
            this.Controls.Add(this.axWebBrowser1);
            this.Controls.Add(this.btn_auth);
            this.Controls.Add(this.btn_upload);
            this.Controls.Add(this.txb_path);
            this.Controls.Add(this.btn_open);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.axWebBrowser1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_open;
        private System.Windows.Forms.TextBox txb_path;
        private System.Windows.Forms.Button btn_upload;
        private System.Windows.Forms.Button btn_auth;
        private AxSHDocVw.AxWebBrowser axWebBrowser1;
        private System.Windows.Forms.ComboBox cb_blog;
        private System.Windows.Forms.Button btn_backup;
    }
}

