using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace NGUYEN_VU_NHAT_MINH_24810320314_BAI_TAP_5_3
{
    public partial class Form1 : Form
    {
        // 1. Định nghĩa List<Product> và BindingSource nguồn
        private List<Product> listProducts = new List<Product>();
        private BindingSource bindingSource = new BindingSource();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cboDanhMuc.SelectedIndex = 0;

            // Nạp dữ liệu mẫu ban đầu
            listProducts.Add(new Product("SP01", "iPhone 15 Pro Max", 32990000, 10, "Điện thoại"));
            listProducts.Add(new Product("SP02", "Laptop Dell XPS 13", 28500000, 5, "Laptop"));
            listProducts.Add(new Product("SP03", "Chuột Logitech MX Master 3S", 2450000, 20, "Phụ kiện"));

            // Gán dữ liệu vào BindingSource và gán lên DataGridView
            LamMoiGrid(listProducts);
            CaiDatTieuDeDgv();
        }

        private void CaiDatTieuDeDgv()
        {
            if (dgvProducts.Columns["ProductId"] != null) dgvProducts.Columns["ProductId"].HeaderText = "Mã SP";
            if (dgvProducts.Columns["ProductName"] != null) dgvProducts.Columns["ProductName"].HeaderText = "Tên sản phẩm";
            if (dgvProducts.Columns["UnitPrice"] != null) dgvProducts.Columns["UnitPrice"].HeaderText = "Đơn giá (VNĐ)";
            if (dgvProducts.Columns["Quantity"] != null) dgvProducts.Columns["Quantity"].HeaderText = "Số lượng";
            if (dgvProducts.Columns["Category"] != null) dgvProducts.Columns["Category"].HeaderText = "Danh mục";
        }

        private void LamMoiGrid(List<Product> dataSource)
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = dataSource;
            dgvProducts.DataSource = bindingSource;
            bindingSource.ResetBindings(false);
        }

        // 2. Nút THÊM SẢN PHẨM
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!KiemTraHopLe()) return;

            string maSP = txtMaSP.Text.Trim();

            // Kiểm tra trùng Mã SP
            if (listProducts.Any(p => p.ProductId.Equals(maSP, StringComparison.OrdinalIgnoreCase)))
            {
                epCheck.SetError(txtMaSP, "Mã sản phẩm này đã tồn tại trong danh sách!");
                MessageBox.Show("Mã sản phẩm đã tồn tại, vui lòng nhập mã khác!", "Thông báo trùng mã", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Product pNew = new Product(
                maSP,
                txtTenSP.Text.Trim(),
                double.Parse(txtDonGia.Text.Trim()),
                int.Parse(txtSoLuong.Text.Trim()),
                cboDanhMuc.SelectedItem.ToString()
            );

            listProducts.Add(pNew);
            LamMoiGrid(listProducts);

            MessageBox.Show($"Thêm thành công sản phẩm [{pNew.ProductName}]!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            XoaFormInput();
        }

        // 3. Click vào dòng trong DataGridView -> Đẩy ngược dữ liệu lên các ô nhập
        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvProducts.Rows[e.RowIndex].DataBoundItem is Product p)
            {
                txtMaSP.Text = p.ProductId;
                txtTenSP.Text = p.ProductName;
                txtDonGia.Text = p.UnitPrice.ToString();
                txtSoLuong.Text = p.Quantity.ToString();
                cboDanhMuc.SelectedItem = p.Category;

                // Khóa không cho sửa Mã SP
                txtMaSP.ReadOnly = true;
            }
        }

        // 4. Nút SỬA SẢN PHẨM
        private void btnSua_Click(object sender, EventArgs e)
        {
            string maSP = txtMaSP.Text.Trim();
            Product pSelect = listProducts.FirstOrDefault(p => p.ProductId.Equals(maSP, StringComparison.OrdinalIgnoreCase));

            if (pSelect == null)
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm từ bảng để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!KiemTraHopLe()) return;

            pSelect.ProductName = txtTenSP.Text.Trim();
            pSelect.UnitPrice = double.Parse(txtDonGia.Text.Trim());
            pSelect.Quantity = int.Parse(txtSoLuong.Text.Trim());
            pSelect.Category = cboDanhMuc.SelectedItem.ToString();

            LamMoiGrid(listProducts);
            MessageBox.Show("Cập nhật thông tin sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            XoaFormInput();
        }

        // 5. Nút XÓA SẢN PHẨM
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow != null && dgvProducts.CurrentRow.DataBoundItem is Product pSelect)
            {
                DialogResult dr = MessageBox.Show($"Bạn có chắc chắn muốn xóa sản phẩm [{pSelect.ProductName}] (Mã: {pSelect.ProductId})?",
                                                  "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dr == DialogResult.Yes)
                {
                    listProducts.Remove(pSelect);
                    LamMoiGrid(listProducts);
                    MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    XoaFormInput();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa trên bảng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // 6. Nút TÌM KIẾM THEO TÊN SP
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string kw = txtTimKiem.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(kw))
            {
                LamMoiGrid(listProducts);
            }
            else
            {
                List<Product> filtered = listProducts.Where(p => p.ProductName.ToLower().Contains(kw)).ToList();
                LamMoiGrid(filtered);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            XoaFormInput();
            txtTimKiem.Clear();
            LamMoiGrid(listProducts);
        }

        private void XoaFormInput()
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            txtDonGia.Clear();
            txtSoLuong.Clear();
            cboDanhMuc.SelectedIndex = 0;
            txtMaSP.ReadOnly = false;
            epCheck.Clear();
            txtMaSP.Focus();
        }

        private bool KiemTraHopLe()
        {
            epCheck.Clear();
            bool valid = true;

            if (string.IsNullOrWhiteSpace(txtMaSP.Text))
            {
                epCheck.SetError(txtMaSP, "Mã SP không được để trống!");
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(txtTenSP.Text))
            {
                epCheck.SetError(txtTenSP, "Tên SP không được để trống!");
                valid = false;
            }

            if (!double.TryParse(txtDonGia.Text.Trim(), out double price) || price < 0)
            {
                epCheck.SetError(txtDonGia, "Đơn giá phải là số dương!");
                valid = false;
            }

            if (!int.TryParse(txtSoLuong.Text.Trim(), out int qty) || qty < 0)
            {
                epCheck.SetError(txtSoLuong, "Số lượng phải là số nguyên dương!");
                valid = false;
            }

            return valid;
        }
    }
}