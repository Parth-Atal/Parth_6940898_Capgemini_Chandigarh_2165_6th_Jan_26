// ============================================================
//  auth.js — Shared Auth + UI Utilities
// ============================================================

function getUserInfo() {
  const raw = localStorage.getItem('user_info');
  return raw ? JSON.parse(raw) : null;
}

/**
 * Require authentication. Optionally restrict to specific roles.
 * Redirects to login if not authenticated or not authorised.
 */
function requireAuth(allowedRoles = null) {
  const token = localStorage.getItem('jwt_token');
  const userInfo = getUserInfo();

  if (!token || !userInfo) {
    window.location.href = 'login.html';
    throw new Error('Not authenticated');
  }

  // Check token expiry
  if (userInfo.expiresAt && new Date(userInfo.expiresAt) < new Date()) {
    logout();
    throw new Error('Token expired');
  }

  // Check role restriction
  if (allowedRoles && !allowedRoles.includes(userInfo.role)) {
    showAlert(`Access denied. Required roles: ${allowedRoles.join(', ')}`, 'error');
    setTimeout(() => window.location.href = 'courses.html', 1500);
    throw new Error('Insufficient role');
  }

  return userInfo;
}

function logout() {
  localStorage.removeItem('jwt_token');
  localStorage.removeItem('user_info');
  window.location.href = 'login.html';
}

// ============================================================
// FORM VALIDATION HELPERS
// ============================================================
function isValidEmail(email) {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}

function showFieldError(elementId, message) {
  const el = document.getElementById(elementId);
  if (el) {
    el.textContent = message;
    el.classList.add('visible');
    el.closest('.form-group')?.classList.add('has-error');
  }
}

function clearErrors() {
  document.querySelectorAll('.field-error').forEach(el => {
    el.textContent = '';
    el.classList.remove('visible');
  });
  document.querySelectorAll('.form-group.has-error').forEach(el => {
    el.classList.remove('has-error');
  });
  const alertBox = document.getElementById('alert-box');
  if (alertBox) alertBox.classList.add('hidden');
}

// ============================================================
// ALERT BANNER
// ============================================================
function showAlert(message, type = 'info') {
  const box = document.getElementById('alert-box');
  if (!box) return;
  box.textContent = message;
  box.className = `alert alert-${type}`;
  box.classList.remove('hidden');
  if (type === 'success') {
    setTimeout(() => box.classList.add('hidden'), 4000);
  }
  box.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
}

// ============================================================
// PASSWORD TOGGLE
// ============================================================
function togglePassword(inputId, btn) {
  const input = document.getElementById(inputId);
  if (input.type === 'password') {
    input.type = 'text';
    btn.textContent = '🙈';
  } else {
    input.type = 'password';
    btn.textContent = '👁';
  }
}
