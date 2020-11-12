using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ProjectZero3_Translation
{
    class Editor
    {
        private static Editor _Instance;
        private EditorUI _Editor;
        protected Editor()
        {
            _Editor = new EditorUI();
            _Editor.dataGridView.DefaultCellStyle.Font = new Font("Consolas", 8.5F);
        }
        public static Editor Instance()
        {
            if (_Instance == null)
            {
                _Instance = new Editor();
            }
            return _Instance;
        }
        public EditorUI TransferData(BlockText data)
        {
            if (_Editor.labelIndex.Text == $"{data.Index}") return _Editor;
            _Editor.labelOffset.Text = $"{data.PointerOffset.ToString("X")}";
            _Editor.labelIndex.Text = $"{data.Index}";
            _Editor.dataGridView.DataSource = null;
            _Editor.dataGridView.Rows.Clear();
            for (int i = 0; i < data.Strings.Count; i++)
            {
                _Editor.dataGridView.Rows.Add($"{i}", data.Strings[i]);
            }
            return _Editor;
        }

        public bool isVisible()
        {
            return _Editor.Visible;
        }

        public int GetIndex()
        {
            return int.Parse(_Editor.labelIndex.Text);
        }
    }
}
