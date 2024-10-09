using Lab04_2280601402_NguyenAnKhang.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab04_2280601402_NguyenAnKhang
{
    public partial class Form1 : Form
    {
        QLSV context = new QLSV();
        public Form1()
        {
            InitializeComponent();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                QLSV context = new QLSV();
                List<Khoa> listFacultys = context.Khoas.ToList();
                List<SinhVien> sinhViens = context.SinhViens.ToList();
                FillFacultyCombobox(listFacultys);
                BindGrid(sinhViens);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BindGrid(List<SinhVien> sinhViens)
        {
            dgvDSSV.Rows.Clear();
            foreach (var s in sinhViens)
            {
                int index = dgvDSSV.Rows.Add();
                dgvDSSV.Rows[index].Cells[0].Value = s.StudentID;
                dgvDSSV.Rows[index].Cells[1].Value = s.FullName;
                dgvDSSV.Rows[index].Cells[2].Value = s.Khoa.FacultyName;
                dgvDSSV.Rows[index].Cells[3].Value = s.AverageScore;
            }
        }

        private void FillFacultyCombobox(List<Khoa> listFacultys)
        {
            this.cbbKhoa.DataSource = listFacultys;
            this.cbbKhoa.DisplayMember = "FacultyName";
            this.cbbKhoa.ValueMember = "FacultyID";
        }
        private void LoadData()
        {
            List<SinhVien> sinhViens = context.SinhViens.ToList();
            BindGrid(sinhViens);
        }


        private void btnThem_Click(object sender, EventArgs e)
        {


            try
            {
                // Kiểm tra xem người dùng đã nhập đủ thông tin chưa
                if (string.IsNullOrWhiteSpace(txtMSSV.Text) ||
                    string.IsNullOrWhiteSpace(txtHoten.Text) ||
                    string.IsNullOrWhiteSpace(txtDiemTB.Text) ||
                    cbbKhoa.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng nhập đủ thông tin: MSSV, Họ tên, Điểm trung bình và Khoa.");
                    return;
                }

                string studentId = txtMSSV.Text;

                // Kiểm tra xem MSSV đã tồn tại chưa
                if (context.SinhViens.Any(s => s.StudentID == studentId))
                {
                    MessageBox.Show("Mã số sinh viên đã tồn tại. Vui lòng nhập mã khác.");
                    return;
                }

                SinhVien newStudent = new SinhVien
                {
                    StudentID = studentId,
                    FullName = txtHoten.Text,
                    AverageScore = float.Parse(txtDiemTB.Text),
                    FacultyID = (int)cbbKhoa.SelectedValue
                };

                context.SinhViens.Add(newStudent);
                context.SaveChanges();
                MessageBox.Show("Thêm sinh viên thành công.");
                LoadData(); // Tải lại dữ liệu để cập nhật grid
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }


        private void btnSua_Click(object sender, EventArgs e)
        {


            try
            {
                // Kiểm tra xem người dùng đã chọn hàng nào trong DataGridView chưa
                if (dgvDSSV.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn một sinh viên để sửa thông tin.");
                    return;
                }

                // Kiểm tra xem người dùng đã nhập đủ thông tin chưa
                if (string.IsNullOrWhiteSpace(txtHoten.Text) ||
                    string.IsNullOrWhiteSpace(txtDiemTB.Text) ||
                    cbbKhoa.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng nhập đủ thông tin: Họ tên, Điểm trung bình và Khoa.");
                    return;
                }

                var selectedRow = dgvDSSV.SelectedRows[0];
                string studentId = selectedRow.Cells[0].Value.ToString(); // MSSV của sinh viên đã chọn

                var student = context.SinhViens.FirstOrDefault(s => s.StudentID == studentId);
                if (student != null)
                {
                    // Cập nhật các trường thông tin khác
                    student.FullName = txtHoten.Text;
                    student.AverageScore = float.Parse(txtDiemTB.Text);
                    student.FacultyID = (int)cbbKhoa.SelectedValue;

                    context.SaveChanges();
                    MessageBox.Show("Sửa thông tin sinh viên thành công.");
                    LoadData(); // Tải lại dữ liệu để cập nhật grid
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }


        private void dgvDSSV_CellClick(object sender, DataGridViewCellEventArgs e)
        {


            if (e.RowIndex >= 0)
            {
                var selectedRow = dgvDSSV.Rows[e.RowIndex];
                txtMSSV.Text = selectedRow.Cells[0].Value.ToString(); // MSSV
                txtHoten.Text = selectedRow.Cells[1].Value.ToString(); // Họ tên
                txtDiemTB.Text = selectedRow.Cells[3].Value.ToString(); // Điểm trung bình

                // Cập nhật combobox với FacultyID tương ứng
                var selectedKhoaName = selectedRow.Cells[2].Value.ToString();
                var selectedKhoa = context.Khoas.FirstOrDefault(k => k.FacultyName == selectedKhoaName);

                if (selectedKhoa != null)
                {
                    cbbKhoa.SelectedValue = selectedKhoa.FacultyID; // Cập nhật combobox
                }
                else
                {
                    MessageBox.Show("Không tìm thấy Khoa tương ứng.");
                }
            }



        }

        private void btnXoa_Click(object sender, EventArgs e)
        {

            try
            {
                // Kiểm tra xem người dùng đã chọn hàng nào trong DataGridView chưa
                if (dgvDSSV.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn một sinh viên để xóa.");
                    return;
                }

                var selectedRow = dgvDSSV.SelectedRows[0];
                string studentId = selectedRow.Cells[0].Value.ToString(); // MSSV của sinh viên đã chọn

                // Xác nhận hành động xóa
                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này không?",
                                                     "Xác nhận xóa",
                                                     MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    var student = context.SinhViens.FirstOrDefault(s => s.StudentID == studentId);
                    if (student != null)
                    {
                        context.SinhViens.Remove(student);
                        context.SaveChanges();
                        MessageBox.Show("Xóa sinh viên thành công.");
                        LoadData(); // Tải lại dữ liệu để cập nhật grid
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên với MSSV đã chọn.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }


        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn thoát không?",
                                                 "Xác nhận thoát",
                                                 MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

    }
}
