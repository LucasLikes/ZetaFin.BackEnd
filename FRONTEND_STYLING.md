# 🎨 CSS & UI - Componentes Responsivos para ZetaFin

## Login & Register - Estilos Responsivos

### Login.css
```css
/* src/pages/Login.css */

.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', sans-serif;
  padding: 16px;
}

.login-box {
  background: white;
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
  padding: 40px;
  width: 100%;
  max-width: 400px;
  animation: slideUp 0.4s ease-out;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.login-box h1 {
  font-size: 28px;
  color: #667eea;
  margin: 0 0 8px 0;
  text-align: center;
}

.login-box h2 {
  font-size: 20px;
  color: #333;
  margin: 0 0 24px 0;
  text-align: center;
  font-weight: 600;
}

/* Alert */
.alert {
  padding: 12px 16px;
  border-radius: 8px;
  margin-bottom: 20px;
  font-size: 14px;
  line-height: 1.5;
}

.alert-error {
  background-color: #fee;
  color: #c33;
  border-left: 4px solid #c33;
}

.alert-success {
  background-color: #efe;
  color: #3c3;
  border-left: 4px solid #3c3;
}

/* Form Groups */
.form-group {
  margin-bottom: 20px;
  display: flex;
  flex-direction: column;
}

.form-group label {
  font-size: 14px;
  font-weight: 600;
  color: #333;
  margin-bottom: 8px;
}

.form-group input {
  padding: 12px 16px;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  font-size: 16px;
  font-family: inherit;
  transition: all 0.3s ease;
  background: white;
}

.form-group input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.form-group input.error {
  border-color: #c33;
  background-color: #fee;
}

.form-group input:disabled {
  background-color: #f5f5f5;
  cursor: not-allowed;
}

.error-message {
  font-size: 12px;
  color: #c33;
  margin-top: 6px;
  display: flex;
  align-items: center;
  gap: 4px;
}

.error-message::before {
  content: '⚠';
}

/* Buttons */
.btn-login,
.btn-register {
  padding: 12px 24px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  width: 100%;
  margin-bottom: 16px;
}

.btn-login:hover:not(:disabled),
.btn-register:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
}

.btn-login:active:not(:disabled),
.btn-register:active:not(:disabled) {
  transform: translateY(0);
}

.btn-login:disabled,
.btn-register:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Login Footer */
.login-footer,
.register-footer {
  text-align: center;
  margin-top: 24px;
  padding-top: 24px;
  border-top: 1px solid #e0e0e0;
}

.login-footer p,
.register-footer p {
  font-size: 14px;
  color: #666;
  margin: 8px 0;
}

.login-footer a,
.register-footer a {
  color: #667eea;
  text-decoration: none;
  font-weight: 600;
  cursor: pointer;
  transition: color 0.3s ease;
}

.login-footer a:hover,
.register-footer a:hover {
  color: #764ba2;
  text-decoration: underline;
}

/* Responsive */
@media (max-width: 768px) {
  .login-box,
  .register-box {
    padding: 24px;
    border-radius: 8px;
  }

  .login-box h1,
  .register-box h1 {
    font-size: 24px;
  }

  .login-box h2,
  .register-box h2 {
    font-size: 18px;
  }

  .form-group input {
    font-size: 16px; /* Prevent zoom on iOS */
    padding: 14px 16px;
  }
}

@media (max-width: 480px) {
  .login-container,
  .register-container {
    padding: 12px;
  }

  .login-box,
  .register-box {
    padding: 20px;
    border-radius: 12px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
  }

  .form-group {
    margin-bottom: 16px;
  }

  .login-footer,
  .register-footer {
    margin-top: 16px;
    padding-top: 16px;
  }
}
```

### Register.css
```css
/* src/pages/Register.css */

.register-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', sans-serif;
  padding: 16px;
}

.register-box {
  background: white;
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
  padding: 40px;
  width: 100%;
  max-width: 480px;
  animation: slideUp 0.4s ease-out;
}

.register-box h1 {
  font-size: 28px;
  color: #667eea;
  margin: 0 0 8px 0;
  text-align: center;
}

.register-box h2 {
  font-size: 20px;
  color: #333;
  margin: 0 0 24px 0;
  text-align: center;
  font-weight: 600;
}

/* Password strength indicator */
.password-strength {
  margin-top: 8px;
  padding: 12px;
  background: #f5f5f5;
  border-radius: 6px;
  font-size: 12px;
}

.password-strength h4 {
  margin: 0 0 8px 0;
  color: #666;
}

.strength-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.strength-list li {
  padding: 4px 0;
  color: #999;
  display: flex;
  align-items: center;
  gap: 8px;
}

.strength-list li.valid {
  color: #3c3;
}

.strength-list li.valid::before {
  content: '✓';
  color: #3c3;
}

.strength-list li::before {
  content: '○';
}

/* Estilos compartilhados com Login */
.register-box .alert,
.register-box .form-group,
.register-box .form-group label,
.register-box .form-group input,
.register-box .form-group input:focus,
.register-box .form-group input.error,
.register-box .form-group input:disabled,
.register-box .error-message,
.register-box .btn-register,
.register-box .register-footer {
  /* Reutilizar estilos do login */
}

.register-box small {
  display: block;
  margin-top: 8px;
  color: #999;
  font-size: 12px;
  line-height: 1.4;
}

@media (max-width: 768px) {
  .register-box {
    padding: 24px;
  }
}

@media (max-width: 480px) {
  .register-container {
    padding: 12px;
  }

  .register-box {
    padding: 20px;
  }

  .password-strength {
    font-size: 11px;
  }

  .register-box small {
    font-size: 11px;
  }
}
```

---

## Dashboard - Estilos Responsivos

### Dashboard.css
```css
/* src/pages/Dashboard.css */

.dashboard {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  background-color: #f5f7fa;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', sans-serif;
}

/* Header */
.dashboard-header {
  background: white;
  border-bottom: 1px solid #e0e0e0;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
  position: sticky;
  top: 0;
  z-index: 100;
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 24px;
  max-width: 1400px;
  margin: 0 auto;
  width: 100%;
}

.logo h1 {
  margin: 0;
  font-size: 24px;
  color: #667eea;
  font-weight: 700;
}

.nav-items {
  display: flex;
  gap: 24px;
  align-items: center;
}

.nav-items a {
  color: #666;
  text-decoration: none;
  font-weight: 500;
  transition: color 0.3s ease;
  border-bottom: 2px solid transparent;
  padding-bottom: 4px;
}

.nav-items a:hover,
.nav-items a.active {
  color: #667eea;
  border-bottom-color: #667eea;
}

.user-menu {
  position: relative;
}

.user-button {
  display: flex;
  align-items: center;
  gap: 12px;
  background: #f0f2f5;
  border: none;
  border-radius: 8px;
  padding: 8px 12px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.user-button:hover {
  background: #e4e6eb;
}

.user-name {
  font-weight: 600;
  color: #333;
  font-size: 14px;
}

.user-initial {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-radius: 50%;
  font-weight: 600;
  font-size: 14px;
}

.dropdown-menu {
  position: absolute;
  top: 100%;
  right: 0;
  background: white;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  min-width: 200px;
  margin-top: 8px;
  overflow: hidden;
  animation: slideDown 0.2s ease-out;
}

@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-8px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.dropdown-menu a,
.dropdown-menu button {
  display: block;
  width: 100%;
  padding: 12px 16px;
  background: none;
  border: none;
  text-align: left;
  color: #333;
  cursor: pointer;
  transition: background 0.3s ease;
  font-size: 14px;
  border-bottom: 1px solid #f0f2f5;
}

.dropdown-menu a:last-child,
.dropdown-menu button:last-child {
  border-bottom: none;
}

.dropdown-menu a:hover,
.dropdown-menu button:hover {
  background: #f5f7fa;
  color: #667eea;
}

.dropdown-menu button.logout-all {
  color: #c33;
}

.dropdown-menu button.logout-all:hover {
  background: #fee;
}

/* Main Content */
.dashboard-content {
  flex: 1;
  padding: 24px;
  max-width: 1400px;
  margin: 0 auto;
  width: 100%;
}

.welcome-section {
  margin-bottom: 32px;
}

.welcome-section h2 {
  font-size: 28px;
  color: #333;
  margin: 0 0 8px 0;
}

.user-email {
  color: #999;
  font-size: 14px;
  margin: 0;
}

/* Sessions Section */
.sessions-section {
  margin-bottom: 32px;
}

.sessions-section h3 {
  font-size: 18px;
  color: #333;
  margin: 0 0 16px 0;
  font-weight: 600;
}

.sessions-list {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 16px;
}

.session-card {
  background: white;
  border-radius: 8px;
  padding: 16px;
  border: 1px solid #e0e0e0;
  display: flex;
  justify-content: space-between;
  align-items: start;
  gap: 16px;
  transition: all 0.3s ease;
}

.session-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  border-color: #667eea;
}

.session-info {
  flex: 1;
}

.session-device {
  display: flex;
  align-items: baseline;
  gap: 8px;
  margin-bottom: 8px;
}

.session-device strong {
  font-size: 16px;
  color: #333;
}

.device-type {
  font-size: 12px;
  background: #f0f2f5;
  color: #666;
  padding: 2px 8px;
  border-radius: 4px;
}

.session-details {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.session-details small {
  color: #999;
  font-size: 12px;
}

.btn-terminate {
  padding: 8px 16px;
  background: #fee;
  color: #c33;
  border: 1px solid #c33;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  white-space: nowrap;
}

.btn-terminate:hover {
  background: #c33;
  color: white;
}

/* Features Section */
.features-section {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 16px;
}

.feature-card {
  background: white;
  border-radius: 8px;
  padding: 24px;
  border: 1px solid #e0e0e0;
  display: flex;
  flex-direction: column;
  transition: all 0.3s ease;
}

.feature-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12);
  border-color: #667eea;
}

.feature-card h3 {
  font-size: 18px;
  color: #333;
  margin: 0 0 8px 0;
}

.feature-card p {
  color: #999;
  font-size: 14px;
  margin: 0 0 16px 0;
  flex: 1;
}

.feature-card button {
  padding: 10px 16px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 6px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  align-self: flex-start;
}

.feature-card button:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

.loading {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  font-size: 18px;
  color: #666;
}

/* Responsive */
@media (max-width: 1024px) {
  .header-content {
    padding: 12px 16px;
  }

  .nav-items {
    gap: 16px;
  }

  .dashboard-content {
    padding: 16px;
  }

  .sessions-list {
    grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  }

  .features-section {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .header-content {
    flex-wrap: wrap;
    gap: 12px;
  }

  .nav-items {
    display: none;
  }

  .logo h1 {
    font-size: 20px;
  }

  .dashboard-content {
    padding: 12px;
  }

  .welcome-section h2 {
    font-size: 24px;
  }

  .sessions-list {
    grid-template-columns: 1fr;
  }

  .features-section {
    grid-template-columns: 1fr;
  }

  .session-card {
    flex-direction: column;
  }

  .btn-terminate {
    width: 100%;
    text-align: center;
  }
}

@media (max-width: 480px) {
  .header-content {
    padding: 12px;
  }

  .user-button {
    padding: 6px 8px;
  }

  .user-name {
    display: none;
  }

  .user-initial {
    width: 28px;
    height: 28px;
    font-size: 12px;
  }

  .logo h1 {
    font-size: 18px;
  }

  .dashboard-content {
    padding: 8px;
  }

  .welcome-section {
    margin-bottom: 20px;
  }

  .welcome-section h2 {
    font-size: 20px;
  }

  .sessions-section h3,
  .sessions-section h3 {
    font-size: 16px;
    margin-bottom: 12px;
  }

  .feature-card {
    padding: 16px;
  }

  .feature-card h3 {
    font-size: 16px;
  }
}
```

---

## Componentes Reutilizáveis

### Alert Component
```css
/* src/components/Alert.module.css */

.alert {
  padding: 12px 16px;
  border-radius: 8px;
  margin-bottom: 16px;
  display: flex;
  gap: 12px;
  align-items: flex-start;
  animation: slideIn 0.3s ease-out;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateX(-20px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

.alert.error {
  background-color: #fee;
  color: #c33;
  border-left: 4px solid #c33;
}

.alert.success {
  background-color: #efe;
  color: #3c3;
  border-left: 4px solid #3c3;
}

.alert.warning {
  background-color: #ffe;
  color: #c93;
  border-left: 4px solid #c93;
}

.alert.info {
  background-color: #eef;
  color: #33c;
  border-left: 4px solid #33c;
}

.icon {
  flex-shrink: 0;
  font-size: 18px;
  line-height: 1.4;
}

.message {
  flex: 1;
  font-size: 14px;
  line-height: 1.5;
}
```

### Button Styles
```css
/* src/components/Button.module.css */

.button {
  padding: 12px 24px;
  border: none;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  font-family: inherit;
  display: inline-flex;
  align-items: center;
  gap: 8px;
}

.button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.primary:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
}

.secondary {
  background: #f0f2f5;
  color: #333;
  border: 1px solid #e0e0e0;
}

.secondary:hover:not(:disabled) {
  background: #e4e6eb;
}

.danger {
  background: #fee;
  color: #c33;
  border: 1px solid #c33;
}

.danger:hover:not(:disabled) {
  background: #c33;
  color: white;
}

.small {
  padding: 8px 16px;
  font-size: 12px;
}

.large {
  padding: 16px 32px;
  font-size: 16px;
}

.full-width {
  width: 100%;
}
```

---

## Variáveis CSS Reutilizáveis

```css
/* src/styles/variables.css */

:root {
  /* Colors */
  --primary: #667eea;
  --primary-dark: #764ba2;
  --secondary: #f0f2f5;
  --success: #3c3;
  --error: #c33;
  --warning: #c93;
  --info: #33c;
  --text-primary: #333;
  --text-secondary: #666;
  --text-muted: #999;
  --border: #e0e0e0;
  --bg-light: #f5f7fa;

  /* Spacing */
  --sp-xs: 4px;
  --sp-sm: 8px;
  --sp-md: 16px;
  --sp-lg: 24px;
  --sp-xl: 32px;
  --sp-2xl: 48px;

  /* Border Radius */
  --radius-sm: 4px;
  --radius-md: 8px;
  --radius-lg: 12px;

  /* Shadows */
  --shadow-sm: 0 2px 4px rgba(0, 0, 0, 0.05);
  --shadow-md: 0 4px 12px rgba(0, 0, 0, 0.1);
  --shadow-lg: 0 10px 40px rgba(0, 0, 0, 0.2);

  /* Font sizes */
  --text-xs: 12px;
  --text-sm: 14px;
  --text-md: 16px;
  --text-lg: 18px;
  --text-xl: 20px;
  --text-2xl: 24px;
  --text-3xl: 28px;

  /* Transitions */
  --transition-fast: 0.2s ease;
  --transition-normal: 0.3s ease;
  --transition-slow: 0.4s ease;
}
```

---

## Mobile-First Breakpoints

```css
/* Mobile First */
@media (min-width: 480px) {
  /* Small tablets */
}

@media (min-width: 768px) {
  /* Tablets */
}

@media (min-width: 1024px) {
  /* Desktops */
}

@media (min-width: 1280px) {
  /* Large desktops */
}

@media (min-width: 1536px) {
  /* Extra large */
}

/* Impressoras */
@media print {
  .no-print {
    display: none;
  }
}

/* Dark mode */
@media (prefers-color-scheme: dark) {
  :root {
    --text-primary: #eee;
    --text-secondary: #ccc;
    --bg-light: #1a1a1a;
  }
}

/* Reduz animações se preferência */
@media (prefers-reduced-motion: reduce) {
  * {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
  }
}
```