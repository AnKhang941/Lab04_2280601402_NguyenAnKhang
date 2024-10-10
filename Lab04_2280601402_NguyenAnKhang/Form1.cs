using Lab04_2280601402_NguyenAnKhang.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
                if (string.IsNullOrWhiteSpace(txtMSSV.Text) ||
                    string.IsNullOrWhiteSpace(txtHoten.Text) ||
                    string.IsNullOrWhiteSpace(txtDiemTB.Text) ||
                    cbbKhoa.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng nhập đủ thông tin: MSSV, Họ tên, Điểm trung bình và Khoa.");
                    return;
                }

                string studentId = txtMSSV.Text;

                if (context.SinhViens.Any(s => s.StudentID == studentId))
                {
                    MessageBox.Show("Mã số sinh viên đã tồn tại. Vui lòng nhập mã khác.");
                    return;
                }

                if (!float.TryParse(txtDiemTB.Text, out float avgScore))
                {
                    MessageBox.Show("Điểm trung bình phải là một số hợp lệ.");
                    return;
                }

                SinhVien newStudent = new SinhVien
                {
                    StudentID = studentId,
                    FullName = txtHoten.Text,
                    AverageScore = avgScore,
                    FacultyID = (int)cbbKhoa.SelectedValue
                };

                context.SinhViens.Add(newStudent);
                context.SaveChanges();
                MessageBox.Show("Thêm sinh viên thành công.");
                LoadData();
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
                if (dgvDSSV.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn một sinh viên để sửa thông tin.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtHoten.Text) ||
                    string.IsNullOrWhiteSpace(txtDiemTB.Text) ||
                    cbbKhoa.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng nhập đủ thông tin: Họ tên, Điểm trung bình và Khoa.");
                    return;
                }

                if (!float.TryParse(txtDiemTB.Text, out float avgScore))
                {
                    MessageBox.Show("Điểm trung bình phải là một số hợp lệ.");
                    return;
                }

                var selectedRow = dgvDSSV.SelectedRows[0];
                string studentId = selectedRow.Cells[0].Value.ToString();

                var student = context.SinhViens.FirstOrDefault(s => s.StudentID == studentId);
                if (student != null)
                {
                    student.FullName = txtHoten.Text;
                    student.AverageScore = avgScore;
                    student.FacultyID = (int)cbbKhoa.SelectedValue;

                    context.SaveChanges();
                    MessageBox.Show("Sửa thông tin sinh viên thành công.");
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvDSSV.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn một sinh viên để xóa.");
                    return;
                }

                var selectedRow = dgvDSSV.SelectedRows[0];
                string studentId = selectedRow.Cells[0].Value.ToString();

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
                        LoadData();
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

        private void dgvDSSV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedRow = dgvDSSV.Rows[e.RowIndex];
                txtMSSV.Text = selectedRow.Cells[0].Value.ToString();
                txtHoten.Text = selectedRow.Cells[1].Value.ToString();
                txtDiemTB.Text = selectedRow.Cells[3].Value.ToString();

                var selectedKhoaName = selectedRow.Cells[2].Value.ToString();
                var selectedKhoa = context.Khoas.FirstOrDefault(k => k.FacultyName == selectedKhoaName);

                if (selectedKhoa != null)
                {
                    cbbKhoa.SelectedValue = selectedKhoa.FacultyID;
                }
                else
                {
                    MessageBox.Show("Không tìm thấy Khoa tương ứng.");
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
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
    }
}
