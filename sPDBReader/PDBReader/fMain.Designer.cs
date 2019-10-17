namespace PDBReader
{
    partial class fMain
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fMain));
            this.pnButton = new System.Windows.Forms.Panel();
            this.btHc = new System.Windows.Forms.Button();
            this.btNear = new System.Windows.Forms.Button();
            this.btDetails = new System.Windows.Forms.Button();
            this.btAfter = new System.Windows.Forms.Button();
            this.btBefore = new System.Windows.Forms.Button();
            this.btOpen = new System.Windows.Forms.Button();
            this.btConvert = new System.Windows.Forms.Button();
            this.lbFile = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbPDBFile = new System.Windows.Forms.ComboBox();
            this.cbConvertFile = new System.Windows.Forms.ComboBox();
            this.pnVisualization = new System.Windows.Forms.Panel();
            this.lbScale = new System.Windows.Forms.Label();
            this.tbScale = new System.Windows.Forms.NumericUpDown();
            this.tbDis = new System.Windows.Forms.NumericUpDown();
            this.lbDis = new System.Windows.Forms.Label();
            this.cbView = new System.Windows.Forms.CheckBox();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.lbFilter = new System.Windows.Forms.Label();
            this.cbHcOrder = new System.Windows.Forms.CheckBox();
            this.cbOutput = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbChainMark = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtResId = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.txtResidue = new System.Windows.Forms.TextBox();
            this.cbInstante = new System.Windows.Forms.CheckBox();
            this.cbBP = new System.Windows.Forms.CheckBox();
            this.pnButton.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDis)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtResId)).BeginInit();
            this.SuspendLayout();
            // 
            // pnButton
            // 
            this.pnButton.Controls.Add(this.btHc);
            this.pnButton.Controls.Add(this.btNear);
            this.pnButton.Controls.Add(this.btDetails);
            this.pnButton.Controls.Add(this.btAfter);
            this.pnButton.Controls.Add(this.btBefore);
            this.pnButton.Controls.Add(this.btOpen);
            this.pnButton.Controls.Add(this.btConvert);
            this.pnButton.Location = new System.Drawing.Point(0, 155);
            this.pnButton.Name = "pnButton";
            this.pnButton.Size = new System.Drawing.Size(514, 49);
            this.pnButton.TabIndex = 0;
            // 
            // btHc
            // 
            this.btHc.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btHc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btHc.Location = new System.Drawing.Point(183, 14);
            this.btHc.Name = "btHc";
            this.btHc.Size = new System.Drawing.Size(39, 23);
            this.btHc.TabIndex = 18;
            this.btHc.Text = "HC";
            this.btHc.UseVisualStyleBackColor = true;
            this.btHc.Visible = false;
            this.btHc.Click += new System.EventHandler(this.btHc_Click);
            // 
            // btNear
            // 
            this.btNear.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btNear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btNear.Location = new System.Drawing.Point(325, 14);
            this.btNear.Name = "btNear";
            this.btNear.Size = new System.Drawing.Size(47, 23);
            this.btNear.TabIndex = 17;
            this.btNear.Tag = "";
            this.btNear.Text = "Near";
            this.btNear.UseVisualStyleBackColor = true;
            this.btNear.Visible = false;
            this.btNear.Click += new System.EventHandler(this.btNear_Click);
            // 
            // btDetails
            // 
            this.btDetails.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btDetails.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btDetails.Location = new System.Drawing.Point(293, 14);
            this.btDetails.Name = "btDetails";
            this.btDetails.Size = new System.Drawing.Size(27, 23);
            this.btDetails.TabIndex = 4;
            this.btDetails.Text = "?";
            this.btDetails.UseVisualStyleBackColor = true;
            this.btDetails.Visible = false;
            this.btDetails.Click += new System.EventHandler(this.btDetails_Click);
            // 
            // btAfter
            // 
            this.btAfter.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btAfter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btAfter.Location = new System.Drawing.Point(260, 14);
            this.btAfter.Name = "btAfter";
            this.btAfter.Size = new System.Drawing.Size(27, 23);
            this.btAfter.TabIndex = 3;
            this.btAfter.Text = ">";
            this.btAfter.UseVisualStyleBackColor = true;
            this.btAfter.Visible = false;
            this.btAfter.Click += new System.EventHandler(this.btAfter_Click);
            // 
            // btBefore
            // 
            this.btBefore.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btBefore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btBefore.Location = new System.Drawing.Point(227, 14);
            this.btBefore.Name = "btBefore";
            this.btBefore.Size = new System.Drawing.Size(27, 23);
            this.btBefore.TabIndex = 2;
            this.btBefore.Text = "<";
            this.btBefore.UseVisualStyleBackColor = true;
            this.btBefore.Visible = false;
            this.btBefore.Click += new System.EventHandler(this.btBefore_Click);
            // 
            // btOpen
            // 
            this.btOpen.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btOpen.Location = new System.Drawing.Point(16, 14);
            this.btOpen.Name = "btOpen";
            this.btOpen.Size = new System.Drawing.Size(75, 23);
            this.btOpen.TabIndex = 1;
            this.btOpen.Text = "Open";
            this.btOpen.UseVisualStyleBackColor = true;
            this.btOpen.Click += new System.EventHandler(this.btOpen_Click);
            // 
            // btConvert
            // 
            this.btConvert.Enabled = false;
            this.btConvert.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btConvert.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btConvert.Location = new System.Drawing.Point(399, 14);
            this.btConvert.Name = "btConvert";
            this.btConvert.Size = new System.Drawing.Size(75, 23);
            this.btConvert.TabIndex = 0;
            this.btConvert.Text = "Convert";
            this.btConvert.UseVisualStyleBackColor = true;
            this.btConvert.Click += new System.EventHandler(this.btConvert_Click);
            // 
            // lbFile
            // 
            this.lbFile.AutoSize = true;
            this.lbFile.Location = new System.Drawing.Point(34, 18);
            this.lbFile.Name = "lbFile";
            this.lbFile.Size = new System.Drawing.Size(84, 13);
            this.lbFile.TabIndex = 2;
            this.lbFile.Text = "Select PDB File:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Output File";
            // 
            // cbPDBFile
            // 
            this.cbPDBFile.FormattingEnabled = true;
            this.cbPDBFile.Location = new System.Drawing.Point(124, 15);
            this.cbPDBFile.Name = "cbPDBFile";
            this.cbPDBFile.Size = new System.Drawing.Size(350, 21);
            this.cbPDBFile.TabIndex = 5;
            // 
            // cbConvertFile
            // 
            this.cbConvertFile.FormattingEnabled = true;
            this.cbConvertFile.Location = new System.Drawing.Point(124, 53);
            this.cbConvertFile.Name = "cbConvertFile";
            this.cbConvertFile.Size = new System.Drawing.Size(350, 21);
            this.cbConvertFile.TabIndex = 6;
            // 
            // pnVisualization
            // 
            this.pnVisualization.BackColor = System.Drawing.Color.Silver;
            this.pnVisualization.Location = new System.Drawing.Point(519, 9);
            this.pnVisualization.Name = "pnVisualization";
            this.pnVisualization.Size = new System.Drawing.Size(200, 195);
            this.pnVisualization.TabIndex = 7;
            this.pnVisualization.DoubleClick += new System.EventHandler(this.pnVisualization_DoubleClick);
            this.pnVisualization.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnVisualization_MouseDown);
            this.pnVisualization.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnVisualization_MouseUp);
            // 
            // lbScale
            // 
            this.lbScale.AutoSize = true;
            this.lbScale.Location = new System.Drawing.Point(293, 82);
            this.lbScale.Name = "lbScale";
            this.lbScale.Size = new System.Drawing.Size(34, 13);
            this.lbScale.TabIndex = 8;
            this.lbScale.Text = "Scale";
            // 
            // tbScale
            // 
            this.tbScale.Enabled = false;
            this.tbScale.Location = new System.Drawing.Point(333, 80);
            this.tbScale.Name = "tbScale";
            this.tbScale.Size = new System.Drawing.Size(39, 20);
            this.tbScale.TabIndex = 9;
            this.tbScale.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.tbScale.ValueChanged += new System.EventHandler(this.tbScale_ValueChanged);
            // 
            // tbDis
            // 
            this.tbDis.DecimalPlaces = 2;
            this.tbDis.Location = new System.Drawing.Point(443, 79);
            this.tbDis.Name = "tbDis";
            this.tbDis.Size = new System.Drawing.Size(52, 20);
            this.tbDis.TabIndex = 11;
            this.tbDis.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.tbDis.ValueChanged += new System.EventHandler(this.tbDis_ValueChanged);
            // 
            // lbDis
            // 
            this.lbDis.AutoSize = true;
            this.lbDis.Location = new System.Drawing.Point(378, 82);
            this.lbDis.Name = "lbDis";
            this.lbDis.Size = new System.Drawing.Size(59, 13);
            this.lbDis.TabIndex = 10;
            this.lbDis.Text = "Radium (Å)";
            // 
            // cbView
            // 
            this.cbView.AutoSize = true;
            this.cbView.Location = new System.Drawing.Point(207, 83);
            this.cbView.Name = "cbView";
            this.cbView.Size = new System.Drawing.Size(80, 17);
            this.cbView.TabIndex = 12;
            this.cbView.Text = "View Struct";
            this.cbView.UseVisualStyleBackColor = true;
            this.cbView.CheckedChanged += new System.EventHandler(this.cbView_CheckedChanged);
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(124, 83);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(36, 20);
            this.txtFilter.TabIndex = 13;
            // 
            // lbFilter
            // 
            this.lbFilter.AutoSize = true;
            this.lbFilter.Location = new System.Drawing.Point(62, 86);
            this.lbFilter.Name = "lbFilter";
            this.lbFilter.Size = new System.Drawing.Size(56, 13);
            this.lbFilter.TabIndex = 14;
            this.lbFilter.Text = "AtomFilter:";
            // 
            // cbHcOrder
            // 
            this.cbHcOrder.AutoSize = true;
            this.cbHcOrder.Location = new System.Drawing.Point(207, 131);
            this.cbHcOrder.Name = "cbHcOrder";
            this.cbHcOrder.Size = new System.Drawing.Size(87, 17);
            this.cbHcOrder.TabIndex = 15;
            this.cbHcOrder.Text = "Use hc order";
            this.cbHcOrder.UseVisualStyleBackColor = true;
            this.cbHcOrder.CheckedChanged += new System.EventHandler(this.cbHcOrder_CheckedChanged);
            // 
            // cbOutput
            // 
            this.cbOutput.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbOutput.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbOutput.DisplayMember = "0";
            this.cbOutput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOutput.FormattingEnabled = true;
            this.cbOutput.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cbOutput.Items.AddRange(new object[] {
            "XML",
            "Matrix",
            "JSON",
            "MolConf",
            "xyz",
            "MD-jeep",
            "VirtualNode"});
            this.cbOutput.Location = new System.Drawing.Point(124, 105);
            this.cbOutput.Name = "cbOutput";
            this.cbOutput.Size = new System.Drawing.Size(79, 21);
            this.cbOutput.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(75, 109);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Output:";
            // 
            // cbChainMark
            // 
            this.cbChainMark.AutoSize = true;
            this.cbChainMark.Enabled = false;
            this.cbChainMark.Location = new System.Drawing.Point(304, 107);
            this.cbChainMark.Name = "cbChainMark";
            this.cbChainMark.Size = new System.Drawing.Size(79, 17);
            this.cbChainMark.TabIndex = 18;
            this.cbChainMark.Text = "Chain View";
            this.cbChainMark.UseVisualStyleBackColor = true;
            this.cbChainMark.CheckedChanged += new System.EventHandler(this.cbChainMark_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(293, 133);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "AminoId:";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // txtResId
            // 
            this.txtResId.Location = new System.Drawing.Point(342, 130);
            this.txtResId.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txtResId.Name = "txtResId";
            this.txtResId.Size = new System.Drawing.Size(41, 20);
            this.txtResId.TabIndex = 21;
            this.txtResId.ValueChanged += new System.EventHandler(this.txtResId_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(57, 133);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "AminoFilter:";
            // 
            // txtResidue
            // 
            this.txtResidue.Location = new System.Drawing.Point(124, 130);
            this.txtResidue.Name = "txtResidue";
            this.txtResidue.Size = new System.Drawing.Size(36, 20);
            this.txtResidue.TabIndex = 22;
            // 
            // cbInstante
            // 
            this.cbInstante.AutoSize = true;
            this.cbInstante.Enabled = false;
            this.cbInstante.Location = new System.Drawing.Point(386, 108);
            this.cbInstante.Name = "cbInstante";
            this.cbInstante.Size = new System.Drawing.Size(96, 17);
            this.cbInstante.TabIndex = 24;
            this.cbInstante.Text = "Instant Update";
            this.cbInstante.UseVisualStyleBackColor = true;
            // 
            // cbBP
            // 
            this.cbBP.AutoSize = true;
            this.cbBP.Location = new System.Drawing.Point(207, 107);
            this.cbBP.Name = "cbBP";
            this.cbBP.Size = new System.Drawing.Size(93, 17);
            this.cbBP.TabIndex = 25;
            this.cbBP.Text = "Output  for BP";
            this.cbBP.UseVisualStyleBackColor = true;
            // 
            // fMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(727, 213);
            this.Controls.Add(this.cbChainMark);
            this.Controls.Add(this.cbBP);
            this.Controls.Add(this.cbInstante);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtResidue);
            this.Controls.Add(this.txtResId);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbOutput);
            this.Controls.Add(this.cbHcOrder);
            this.Controls.Add(this.lbFilter);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.cbView);
            this.Controls.Add(this.tbDis);
            this.Controls.Add(this.lbDis);
            this.Controls.Add(this.tbScale);
            this.Controls.Add(this.lbScale);
            this.Controls.Add(this.pnVisualization);
            this.Controls.Add(this.cbConvertFile);
            this.Controls.Add(this.cbPDBFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbFile);
            this.Controls.Add(this.pnButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "fMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HCProt";
            this.SizeChanged += new System.EventHandler(this.fMain_SizeChanged);
            this.pnButton.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDis)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtResId)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnButton;
        private System.Windows.Forms.Button btConvert;
        private System.Windows.Forms.Button btOpen;
        private System.Windows.Forms.Label lbFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbPDBFile;
        private System.Windows.Forms.ComboBox cbConvertFile;
        private System.Windows.Forms.Panel pnVisualization;
        private System.Windows.Forms.Label lbScale;
        private System.Windows.Forms.NumericUpDown tbScale;
        private System.Windows.Forms.NumericUpDown tbDis;
        private System.Windows.Forms.Label lbDis;
        private System.Windows.Forms.Button btAfter;
        private System.Windows.Forms.Button btBefore;
        private System.Windows.Forms.CheckBox cbView;
        private System.Windows.Forms.Button btDetails;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label lbFilter;
        private System.Windows.Forms.CheckBox cbHcOrder;
        private System.Windows.Forms.Button btNear;
        private System.Windows.Forms.Button btHc;
        private System.Windows.Forms.ComboBox cbOutput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbChainMark;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown txtResId;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtResidue;
        private System.Windows.Forms.CheckBox cbInstante;
        private System.Windows.Forms.CheckBox cbBP;
    }
}

