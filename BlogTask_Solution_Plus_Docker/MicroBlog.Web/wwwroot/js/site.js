// site.js

// 1) If you want to submit login via JS instead of a form POST:
document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', async e => {
            e.preventDefault();
            const user = document.getElementById('user').value;
            const pass = document.getElementById('pass').value;
            const resp = await fetch(`${window.apiBase}/auth/login`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username: user, password: pass })
            });
            if (!resp.ok) {
                alert('Login failed');
                return;
            }
            const { token } = await resp.json();
            localStorage.setItem('microblog_jwt_token', token);
            window.location = '/';
        });
    }

    // 2) Timeline fetch
    if (document.getElementById('timeline')) {
        const token = localStorage.getItem('microblog_jwt_token');
        fetch(`${window.apiBase}/posts`, {
            headers: { Authorization: 'Bearer ' + token }
        })
            .then(r => r.json())
            .then(posts => {
                const w = window.innerWidth;
                const container = document.getElementById('timeline');
                posts.forEach(p => {
                    // choose best image URL
                    const imgUrl = p.webPImageUrl || p.originalImageUrl;
                    const card = document.createElement('div');
                    card.className = 'post-card';
                    card.innerHTML = `
            <h5>${p.userName}</h5>
            <p>${p.text}</p>
            ${imgUrl ? `<img src="${imgUrl}" style="max-width:100%">` : ''}
            <small>${new Date(p.createdAt).toLocaleString()}</small>
          `;
                    container.appendChild(card);
                });
            });
    }
});
