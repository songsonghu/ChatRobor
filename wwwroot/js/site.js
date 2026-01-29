// Site.js - General JavaScript utilities

// Format date
function formatDate(date) {
    if (typeof date === 'string') {
        date = new Date(date);
    }
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
}

// Show notification
function showNotification(message, type = 'info') {
    const alert = document.createElement('div');
    alert.className = `alert alert-${type}`;
    alert.textContent = message;
    alert.style.position = 'fixed';
    alert.style.top = '20px';
    alert.style.right = '20px';
    alert.style.zIndex = '1000';
    alert.style.maxWidth = '400px';
    
    document.body.appendChild(alert);
    
    setTimeout(() => {
        alert.remove();
    }, 5000);
}

// AJAX helper
async function ajax(url, options = {}) {
    const defaultOptions = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    };

    const config = { ...defaultOptions, ...options };
    
    try {
        const response = await fetch(url, config);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return await response.json();
    } catch (error) {
        console.error('AJAX Error:', error);
        showNotification('An error occurred: ' + error.message, 'danger');
        throw error;
    }
}

// Confirm dialog
function confirmAction(message = 'Are you sure?') {
    return confirm(message);
}

// Disable button during submission
function disableButtonOnSubmit(form) {
    form.addEventListener('submit', function() {
        const button = form.querySelector('button[type="submit"]');
        if (button) {
            button.disabled = true;
            button.textContent = 'Loading...';
        }
    });
}

// Auto-grow textarea
function autoGrowTextarea(textarea) {
    textarea.style.height = 'auto';
    textarea.style.height = Math.min(textarea.scrollHeight, 200) + 'px';
}

document.addEventListener('DOMContentLoaded', function() {
    // Add auto-grow to all textareas
    document.querySelectorAll('textarea').forEach(textarea => {
        textarea.addEventListener('input', function() {
            autoGrowTextarea(this);
        });
    });

    // Disable form buttons on submit
    document.querySelectorAll('form').forEach(form => {
        disableButtonOnSubmit(form);
    });
});
