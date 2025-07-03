let currentUserId = null;

function openDetailsModal(userId) {
    currentUserId = userId;
    const modal = new bootstrap.Modal(document.getElementById('userDetailsModal'));

    // Show loading
    document.getElementById('userDetailsContent').innerHTML = `
            <div class="text-center">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Đang tải...</span>
                </div>
            </div>
        `;

    modal.show();

    // Simulate API call
    setTimeout(() => {
        loadUserDetails(userId);
    }, 500);
}

function loadUserDetails(userId) {
    var currentUrl = window.location.href;
    $.ajax({
        url: currentUrl + "/Details", // Đường dẫn controller của bạn
        type: 'GET',
        data: { id: userId },
        success: function (response) {
            if (response.success) {
                const user = response.data;

                const content = `
                    <div class="user-detail-card">
                        <div class="row">
                            <div class="col-md-3 text-center">
                                <div class="user-avatar mx-auto">
                                    ${user.avatarUrl
                        ? `<img src="${user.avatarUrl}" alt="Avatar" class="img-fluid rounded-circle mb-3">`
                        : `<div class="avatar-placeholder">${user.fullName.charAt(0).toUpperCase()}</div>`
                    }
                                </div>
                            </div>
                            <div class="col-md-9">
                                <h4 class="mb-3">${user.fullName}</h4>
                                <div class="row">
                                    <div class="col-sm-6">
                                        <strong>Tên đăng nhập:</strong><br>
                                        <span class="text-muted">${user.userName}</span>
                                    </div>
                                    <div class="col-sm-6">
                                        <strong>Email:</strong><br>
                                        <a href="mailto:${user.email}" class="text-decoration-none">${user.email}</a>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-sm-6">
                                        <strong>Số điện thoại:</strong><br>
                                        <span class="text-muted">${user.phoneNumber || 'Chưa cập nhật'}</span>
                                    </div>
                                    <div class="col-sm-6">
                                        <strong>Trạng thái:</strong><br>
                                        <span class="status-badge ${user.isActive ? 'status-active' : 'status-inactive'}">
                                            <i class="fas fa-${user.isActive ? 'check' : 'times'}-circle me-1"></i>
                                            ${user.isActive ? 'Hoạt động' : 'Tạm khóa'}
                                        </span>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-sm-6">
                                        <strong>Ngày tạo:</strong><br>
                                        <span class="text-muted">${new Date(user.createdAt).toLocaleString('vi-VN')}</span>
                                    </div>
                                    <div class="col-sm-6">
                                        <strong>Quyền:</strong><br>
                                        <span class="text-muted">${user.roles.join(', ')}</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                `;

                document.getElementById('userDetailsContent').innerHTML = content;
            } else {
                document.getElementById('userDetailsContent').innerHTML = `<p class="text-danger">Không tìm thấy người dùng.</p>`;
            }
        },
        error: function () {
            document.getElementById('userDetailsContent').innerHTML = `<p class="text-danger">Lỗi khi tải dữ liệu.</p>`;
        }
    });
}


function openCreateModal() {
    currentUserId = null;
    document.getElementById('userFormModalTitle').innerHTML = '<i class="fas fa-plus me-2"></i>Thêm người dùng mới';
    document.getElementById('userForm').reset();
    document.getElementById('userId').value = '';
    document.getElementById('passwordSection').style.display = 'block';
    document.getElementById('password').required = true;
    document.getElementById('confirmPassword').required = true;

    const modal = new bootstrap.Modal(document.getElementById('userFormModal'));
    modal.show();
}

function openEditModal(userId) {
    currentUserId = userId;
    const user = sampleUsers[userId];
    if (!user) return;

    document.getElementById('userFormModalTitle').innerHTML = '<i class="fas fa-edit me-2"></i>Chỉnh sửa người dùng';
    document.getElementById('userId').value = user.id;
    document.getElementById('fullName').value = user.fullName;
    document.getElementById('userName').value = user.userName;
    document.getElementById('email').value = user.email;
    document.getElementById('phoneNumber').value = user.phoneNumber || '';
    document.getElementById('isActive').checked = user.isActive;

    document.getElementById('passwordSection').style.display = 'none';
    document.getElementById('password').required = false;
    document.getElementById('confirmPassword').required = false;

    const modal = new bootstrap.Modal(document.getElementById('userFormModal'));
    modal.show();
}

function openRoleModal(userId) {
    currentUserId = userId;
    const user = sampleUsers[userId];
    if (!user) return;

    // Reset checkboxes
    document.getElementById('adminRole').checked = user.roles.includes('Admin');
    document.getElementById('managerRole').checked = user.roles.includes('Manager');
    document.getElementById('userRole').checked = user.roles.includes('User');

    const modal = new bootstrap.Modal(document.getElementById('roleModal'));
    modal.show();
}

function saveUser() {
    const form = document.getElementById('userForm');
    const formData = new FormData(form);

    // Validate passwords if creating new user
    if (!currentUserId) {
        const password = document.getElementById('password').value;
        const confirmPassword = document.getElementById('confirmPassword').value;

        if (password !== confirmPassword) {
            showAlert('Mật khẩu xác nhận không khớp!', 'error');
            return;
        }
    }

    // Simulate API call
    showAlert('Đang lưu...', 'info');

    setTimeout(() => {
        const modal = bootstrap.Modal.getInstance(document.getElementById('userFormModal'));
        modal.hide();
        showAlert(currentUserId ? 'Cập nhật người dùng thành công!' : 'Thêm người dùng thành công!', 'success');
        // Refresh table here
    }, 1000);
}

function saveUserRoles() {
    const roles = [];
    if (document.getElementById('adminRole').checked) roles.push('Admin');
    if (document.getElementById('managerRole').checked) roles.push('Manager');
    if (document.getElementById('userRole').checked) roles.push('User');

    // Simulate API call
    showAlert('Đang cập nhật quyền...', 'info');

    setTimeout(() => {
        const modal = bootstrap.Modal.getInstance(document.getElementById('roleModal'));
        modal.hide();
        showAlert('Cập nhật quyền thành công!', 'success');
        // Update user roles in sample data
        if (sampleUsers[currentUserId]) {
            sampleUsers[currentUserId].roles = roles;
        }
    }, 1000);
}

function toggleUserStatus(userId, isActive) {
    const action = isActive ? 'khóa' : 'kích hoạt';
    const icon = isActive ? 'warning' : 'question';

    Swal.fire({
        title: `Xác nhận ${action} tài khoản?`,
        text: isActive ? 'Người dùng sẽ không thể đăng nhập.' : 'Người dùng sẽ có thể đăng nhập trở lại.',
        icon: icon,
        showCancelButton: true,
        confirmButtonColor: isActive ? '#ff4b2b' : '#38ef7d',
        cancelButtonColor: '#6c757d',
        confirmButtonText: `Đồng ý ${action}`,
        cancelButtonText: 'Hủy bỏ',
        customClass: {
            popup: 'animate__animated animate__zoomIn'
        }
    }).then((result) => {
        if (result.isConfirmed) {
            // Simulate API call
            Swal.fire({
                title: `Đang ${action} tài khoản...`,
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });

            setTimeout(() => {
                // Update status in sample data
                if (sampleUsers[userId]) {
                    sampleUsers[userId].isActive = !isActive;
                }

                Swal.fire({
                    title: `${action.charAt(0).toUpperCase() + action.slice(1)} tài khoản thành công!`,
                    icon: 'success',
                    timer: 1500,
                    showConfirmButton: false,
                    customClass: {
                        popup: 'animate__animated animate__zoomIn'
                    }
                });

                // Update the table row
                updateTableRow(userId);
            }, 1000);
        }
    });
}

function deleteUser(userId, userName) {
    Swal.fire({
        title: 'Xác nhận xóa người dùng?',
        text: `Bạn có chắc chắn muốn xóa "${userName}"? Hành động này không thể hoàn tác.`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ff4b2b',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Đồng ý xóa',
        cancelButtonText: 'Hủy bỏ',
        customClass: {
            popup: 'animate__animated animate__shakeX'
        }
    }).then((result) => {
        if (result.isConfirmed) {
            // Simulate API call
            Swal.fire({
                title: 'Đang xóa người dùng...',
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });

            setTimeout(() => {
                // Remove from sample data
                delete sampleUsers[userId];

                Swal.fire({
                    title: 'Xóa người dùng thành công!',
                    icon: 'success',
                    timer: 1500,
                    showConfirmButton: false,
                    customClass: {
                        popup: 'animate__animated animate__zoomIn'
                    }
                });

                // Remove table row
                const row = document.querySelector(`tr[data-user-id="${userId}"]`);
                if (row) {
                    row.remove();
                    updateUserCount();
                }
            }, 1000);
        }
    });
}

function searchUsers() {
    const searchTerm = document.getElementById('searchInput').value.toLowerCase();
    const tableBody = document.getElementById('userTableBody');
    const rows = tableBody.querySelectorAll('tr');

    let visibleCount = 0;
    rows.forEach(row => {
        const text = row.textContent.toLowerCase();
        if (text.includes(searchTerm)) {
            row.style.display = '';
            visibleCount++;
        } else {
            row.style.display = 'none';
        }
    });

    document.getElementById('userCount').textContent = visibleCount;

    if (visibleCount === 0 && searchTerm) {
        showNoResultsMessage(searchTerm);
    } else {
        hideNoResultsMessage();
    }
}

function updateTableRow(userId) {
    const user = sampleUsers[userId];
    if (!user) return;

    const row = document.querySelector(`tr[data-user-id="${userId}"]`);
    if (!row) return;

    // Update status badge
    const statusBadge = row.querySelector('.status-badge');
    if (user.isActive) {
        statusBadge.className = 'status-badge status-active';
        statusBadge.innerHTML = '<i class="fas fa-check-circle me-1"></i>Hoạt động';
    } else {
        statusBadge.className = 'status-badge status-inactive';
        statusBadge.innerHTML = '<i class="fas fa-times-circle me-1"></i>Tạm khóa';
    }

    // Update toggle button
    const toggleBtn = row.querySelector('button[onclick*="toggleUserStatus"]');
    toggleBtn.setAttribute('onclick', `toggleUserStatus('${userId}', ${user.isActive})`);
    toggleBtn.setAttribute('title', user.isActive ? 'Khóa tài khoản' : 'Kích hoạt tài khoản');
    toggleBtn.innerHTML = `<i class="fas fa-${user.isActive ? 'lock' : 'unlock'}"></i>`;
}

function updateUserCount() {
    const visibleRows = document.querySelectorAll('#userTableBody tr:not([style*="display: none"])').length;
    document.getElementById('userCount').textContent = visibleRows;
}

function showNoResultsMessage(searchTerm) {
    const tableBody = document.getElementById('userTableBody');
    const existingMessage = tableBody.querySelector('.no-results-row');

    if (!existingMessage) {
        const row = document.createElement('tr');
        row.className = 'no-results-row';
        row.innerHTML = `
                <td colspan="7" class="text-center py-5">
                    <i class="fas fa-search fa-3x text-muted mb-3"></i>
                    <h5 class="text-muted">Không tìm thấy kết quả</h5>
                    <p class="text-muted">Không tìm thấy kết quả cho "<strong>${searchTerm}</strong>"</p>
                </td>
            `;
        tableBody.appendChild(row);
    }
}

function hideNoResultsMessage() {
    const tableBody = document.getElementById('userTableBody');
    const existingMessage = tableBody.querySelector('.no-results-row');
    if (existingMessage) {
        existingMessage.remove();
    }
}

function showAlert(message, type) {
    const alertContainer = document.getElementById('alertContainer');
    const alertClass = type === 'success' ? 'alert-success' : type === 'error' ? 'alert-danger' : 'alert-info';
    const icon = type === 'success' ? 'check-circle' : type === 'error' ? 'exclamation-triangle' : 'info-circle';

    const alert = document.createElement('div');
    alert.className = `alert ${alertClass} alert-dismissible fade show animate__animated animate__fadeInDown`;
    alert.innerHTML = `
            <i class="fas fa-${icon} me-2"></i>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;

    alertContainer.appendChild(alert);

    // Auto hide after 5 seconds
    setTimeout(() => {
        if (alert.parentNode) {
            alert.classList.add('animate__fadeOutUp');
            setTimeout(() => {
                if (alert.parentNode) {
                    alert.remove();
                }
            }, 500);
        }
    }, 5000);
}

// Add data attributes to existing table rows for easier manipulation
document.addEventListener('DOMContentLoaded', function () {
    const rows = document.querySelectorAll('#userTableBody tr');
    rows.forEach((row, index) => {
        row.setAttribute('data-user-id', index + 1);
    });

    // Add search on Enter key
    document.getElementById('searchInput').addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            searchUsers();
        }
    });

    // Real-time search
    document.getElementById('searchInput').addEventListener('input', function () {
        clearTimeout(this.searchTimeout);
        this.searchTimeout = setTimeout(() => {
            searchUsers();
        }, 300);
    });
});

// Enhanced form validation
function validateForm() {
    const form = document.getElementById('userForm');
    const requiredFields = form.querySelectorAll('[required]');
    let isValid = true;

    requiredFields.forEach(field => {
        if (!field.value.trim()) {
            field.classList.add('is-invalid');
            isValid = false;
        } else {
            field.classList.remove('is-invalid');
        }
    });

    // Email validation
    const email = document.getElementById('email');
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (email.value && !emailRegex.test(email.value)) {
        email.classList.add('is-invalid');
        isValid = false;
    }

    // Phone validation
    const phone = document.getElementById('phoneNumber');
    const phoneRegex = /^[0-9+\-\s()]+$/;
    if (phone.value && !phoneRegex.test(phone.value)) {
        phone.classList.add('is-invalid');
        isValid = false;
    }

    return isValid;
}

// Update saveUser function to include validation
function saveUser() {
    if (!validateForm()) {
        showAlert('Vui lòng kiểm tra lại thông tin!', 'error');
        return;
    }

    const form = document.getElementById('userForm');
    const formData = new FormData(form);

    // Validate passwords if creating new user
    if (!currentUserId) {
        const password = document.getElementById('password').value;
        const confirmPassword = document.getElementById('confirmPassword').value;

        if (password !== confirmPassword) {
            document.getElementById('confirmPassword').classList.add('is-invalid');
            showAlert('Mật khẩu xác nhận không khớp!', 'error');
            return;
        }
    }

    // Show loading state
    const saveBtn = document.querySelector('#userFormModal .btn-success');
    const originalText = saveBtn.innerHTML;
    saveBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Đang lưu...';
    saveBtn.disabled = true;

    // Simulate API call
    setTimeout(() => {
        const modal = bootstrap.Modal.getInstance(document.getElementById('userFormModal'));
        modal.hide();

        // Reset button
        saveBtn.innerHTML = originalText;
        saveBtn.disabled = false;

        showAlert(currentUserId ? 'Cập nhật người dùng thành công!' : 'Thêm người dùng thành công!', 'success');

        // If editing, update the table row
        if (currentUserId && sampleUsers[currentUserId]) {
            // Update sample data with form values
            const user = sampleUsers[currentUserId];
            user.fullName = document.getElementById('fullName').value;
            user.userName = document.getElementById('userName').value;
            user.email = document.getElementById('email').value;
            user.phoneNumber = document.getElementById('phoneNumber').value;
            user.isActive = document.getElementById('isActive').checked;

            updateTableRow(currentUserId);
        }
    }, 1500);
}