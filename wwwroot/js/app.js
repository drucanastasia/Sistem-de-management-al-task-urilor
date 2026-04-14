// ===================== STATE =====================
const DB_KEY = 'taskmanagement_db';

function getDB() {
    try { return JSON.parse(localStorage.getItem(DB_KEY)) || { users: [], sessions: {} }; }
    catch { return { users: [], sessions: {} }; }
}
function saveDB(db) { localStorage.setItem(DB_KEY, JSON.stringify(db)); }

function getUser(username) {
    const db = getDB();
    return db.users.find(u => u.username === username);
}

function getCurrentUser() {
    const username = sessionStorage.getItem('tm_user');
    if (!username) return null;
    return getUser(username);
}

function saveUser(user) {
    const db = getDB();
    const idx = db.users.findIndex(u => u.username === user.username);
    if (idx >= 0) db.users[idx] = user;
    else db.users.push(user);
    saveDB(db);
}

function freshUser(username, firstName, lastName) {
    return {
        username,
        firstName: firstName || '',
        lastName: lastName || '',
        password: '',
        xp: 0,
        level: 1,
        tasks: [],
        notes: [],
        calEvents: [],
        theme: 'light',
        createdAt: Date.now()
    };
}

// ===================== AUTH =====================
function switchAuth(mode) {
    document.getElementById('loginForm').style.display = mode === 'login' ? '' : 'none';
    document.getElementById('registerForm').style.display = mode === 'register' ? '' : 'none';
}

function doLogin() {
    const username = document.getElementById('loginUser').value.trim();
    const pass = document.getElementById('loginPass').value;
    const err = document.getElementById('loginError');
    if (!username || !pass) { showErr(err, 'Completează toate câmpurile.'); return; }
    const user = getUser(username);
    if (!user || user.password !== btoa(pass)) { showErr(err, 'Utilizator sau parolă greșite.'); return; }
    err.style.display = 'none';
    sessionStorage.setItem('tm_user', username);
    setTheme(user.theme || 'light');
    enterApp();
}

function doRegister() {
    const first = document.getElementById('regFirst').value.trim();
    const last = document.getElementById('regLast').value.trim();
    const username = document.getElementById('regUser').value.trim();
    const pass = document.getElementById('regPass').value;
    const err = document.getElementById('registerError');
    if (!username || !pass) { showErr(err, 'Utilizatorul și parola sunt obligatorii.'); return; }
    if (pass.length < 6) { showErr(err, 'Parola trebuie să aibă minim 6 caractere.'); return; }
    if (getUser(username)) { showErr(err, 'Acest utilizator există deja.'); return; }
    err.style.display = 'none';
    const user = freshUser(username, first, last);
    user.password = btoa(pass);
    // demo tasks
    user.tasks = [
        { id: uid(), title: 'Bun venit în TaskManagement! 🎉', desc: 'Acesta este primul tău task. Poți să îl muți în Progress sau Done.', cat: 'timp', status: 'todo', createdAt: Date.now(), completedAt: null },
        { id: uid(), title: 'Explorează toate funcțiile', desc: 'Calendar, Notițe, Export, Temă dark...', cat: 'munca', status: 'todo', createdAt: Date.now(), completedAt: null },
    ];
    saveUser(user);
    sessionStorage.setItem('tm_user', username);
    enterApp();
}

function doLogout() {
    sessionStorage.removeItem('tm_user');
    document.getElementById('mainApp').style.display = 'none';
    document.getElementById('authScreen').style.display = 'flex';
    document.getElementById('loginUser').value = '';
    document.getElementById('loginPass').value = '';
    switchAuth('login');
}

function showErr(el, msg) { el.textContent = msg; el.style.display = 'block'; }

function enterApp() {
    document.getElementById('authScreen').style.display = 'none';
    document.getElementById('mainApp').style.display = 'flex';
    loadApp();
    navigate('tasks');
}

// ===================== APP INIT =====================
function uid() { return Date.now().toString(36) + Math.random().toString(36).slice(2); }

function loadApp() {
    const user = getCurrentUser();
    if (!user) return;
    // Sidebar user info
    const initials = ((user.firstName[0] || '') + (user.lastName[0] || user.username[0] || '')).toUpperCase() || 'U';
    const displayName = (user.firstName && user.lastName) ? user.firstName + ' ' + user.lastName : user.username;
    document.getElementById('sidebarAvatar').textContent = initials;
    document.getElementById('sidebarName').textContent = displayName;
    document.getElementById('profileAvatarBig').textContent = initials;
    document.getElementById('profileName').textContent = displayName;
    document.getElementById('profileEmail').textContent = '@' + user.username;
    updateXpUI(user);
}

function updateXpUI(user) {
    const xpToNext = xpForLevel(user.level + 1) - user.xp;
    const xpThisLevel = xpForLevel(user.level);
    const xpNextLevel = xpForLevel(user.level + 1);
    const pct = user.level >= 50 ? 100 : Math.round((user.xp - xpThisLevel) / (xpNextLevel - xpThisLevel) * 100);
    document.getElementById('sidebarLevel').textContent = 'Niv.' + user.level;
    document.getElementById('sidebarXpText').textContent = user.xp + ' XP';
    document.getElementById('sidebarXpFill').style.width = pct + '%';
    document.getElementById('profileLevelNum').textContent = user.level;
    document.getElementById('profileXpCur').textContent = user.xp + ' XP';
    document.getElementById('profileXpNext').textContent = user.level < 50 ? xpToNext + ' XP până la nivel ' + (user.level + 1) : 'Nivel maxim atins!';
    document.getElementById('profileXpFill').style.width = pct + '%';
    document.getElementById('statXp').textContent = user.xp;
    document.getElementById('statLevel').textContent = 'Nivel ' + user.level;
    document.getElementById('pStatXp').textContent = user.xp;
    document.getElementById('pStatLevel').textContent = user.level;
}

function xpForLevel(lvl) {
    // Level 1: 0xp, each level needs lvl*50 xp
    let total = 0;
    for (let i = 1; i < lvl; i++) total += i * 50;
    return total;
}

function calcLevel(xp) {
    let lvl = 1;
    while (lvl < 50 && xp >= xpForLevel(lvl + 1)) lvl++;
    return lvl;
}

// ===================== NAVIGATION =====================
function navigate(page) {
    document.querySelectorAll('.page').forEach(p => p.classList.remove('active'));
    document.querySelectorAll('.nav-item').forEach(n => n.classList.remove('active'));
    document.getElementById('page-' + page).classList.add('active');
    document.getElementById('nav-' + page).classList.add('active');
    if (page === 'tasks') renderTasks();
    if (page === 'calendar') renderCalendar();
    if (page === 'notes') renderNotes();
    if (page === 'profile') renderProfile();
}

// ===================== TASKS =====================
let editingTaskId = null;
let currentFilter = 'all';

function openAddTask() {
    editingTaskId = null;
    document.getElementById('modalTitle').textContent = 'Adaugă task nou';
    document.getElementById('taskTitle').value = '';
    document.getElementById('taskDesc').value = '';
    document.getElementById('taskCategory').value = 'munca';
    document.getElementById('taskStatus').value = 'todo';
    openModal('taskModal');
    setTimeout(() => document.getElementById('taskTitle').focus(), 200);
}

function openEditTask(id) {
    const user = getCurrentUser();
    const task = user.tasks.find(t => t.id === id);
    if (!task) return;
    editingTaskId = id;
    document.getElementById('modalTitle').textContent = 'Editează task';
    document.getElementById('taskTitle').value = task.title;
    document.getElementById('taskDesc').value = task.desc || '';
    document.getElementById('taskCategory').value = task.cat;
    document.getElementById('taskStatus').value = task.status;
    openModal('taskModal');
    setTimeout(() => document.getElementById('taskTitle').focus(), 200);
}

function saveTask() {
    const title = document.getElementById('taskTitle').value.trim();
    if (!title) { document.getElementById('taskTitle').focus(); return; }
    const user = getCurrentUser();
    const cat = document.getElementById('taskCategory').value;
    const status = document.getElementById('taskStatus').value;
    const desc = document.getElementById('taskDesc').value.trim();
    let leveledUp = false;

    if (editingTaskId) {
        const task = user.tasks.find(t => t.id === editingTaskId);
        if (task) {
            const wasNotDone = task.status !== 'done';
            const isNowDone = status === 'done';
            task.title = title; task.desc = desc; task.cat = cat;
            if (wasNotDone && isNowDone) {
                task.status = 'done';
                task.completedAt = Date.now();
                const res = awardXp(user, 10);
                leveledUp = res.leveledUp;
            } else {
                task.status = status;
            }
        }
    } else {
        const task = { id: uid(), title, desc, cat, status, createdAt: Date.now(), completedAt: null };
        if (status === 'done') { task.completedAt = Date.now(); const r = awardXp(user, 10); leveledUp = r.leveledUp; }
        user.tasks.push(task);
    }

    saveUser(user);
    updateXpUI(user);
    closeModal('taskModal');
    renderTasks();

    if (status === 'done' || (editingTaskId && document.getElementById('taskStatus').value === 'done')) {
        showToast(leveledUp ? '🎉 Nivel nou: ' + user.level + '! +10 XP câștigat!' : '✅ Task finalizat! +10 XP', leveledUp ? 'level' : 'success');
    } else {
        showToast(editingTaskId ? '✏️ Task actualizat!' : '➕ Task adăugat!', 'success');
    }
}

function awardXp(user, amount) {
    user.xp += amount;
    const newLevel = calcLevel(user.xp);
    const leveledUp = newLevel > user.level;
    user.level = newLevel;
    return { leveledUp };
}

function moveTask(id, toStatus) {
    const user = getCurrentUser();
    const task = user.tasks.find(t => t.id === id);
    if (!task) return;
    let leveledUp = false;
    if (toStatus === 'done' && task.status !== 'done') {
        task.completedAt = Date.now();
        const r = awardXp(user, 10);
        leveledUp = r.leveledUp;
    }
    task.status = toStatus;
    saveUser(user);
    updateXpUI(user);
    renderTasks();
    if (toStatus === 'done') showToast(leveledUp ? '🎉 Nivel nou: ' + user.level + '! +10 XP!' : '✅ Task finalizat! +10 XP', leveledUp ? 'level' : 'success');
}

function deleteTask(id) {
    showConfirm('Șterge task', 'Ești sigur că vrei să ștergi acest task? Nu poate fi recuperat.', () => {
        const user = getCurrentUser();
        user.tasks = user.tasks.filter(t => t.id !== id);
        saveUser(user);
        renderTasks();
        showToast('🗑️ Task șters.', 'success');
    });
}

function filterTasks(cat, el) {
    currentFilter = cat;
    document.querySelectorAll('#page-tasks .chip').forEach(c => c.classList.remove('active'));
    el.classList.add('active');
    renderTasks();
}

function searchTasks(q) {
    renderTasks(q.toLowerCase());
}

function renderTasks(searchQ = '') {
    const user = getCurrentUser();
    if (!user) return;
    let tasks = user.tasks;
    if (currentFilter !== 'all') tasks = tasks.filter(t => t.cat === currentFilter);
    if (searchQ) tasks = tasks.filter(t => t.title.toLowerCase().includes(searchQ) || (t.desc || '').toLowerCase().includes(searchQ));

    const todo = tasks.filter(t => t.status === 'todo');
    const prog = tasks.filter(t => t.status === 'prog');
    const done = tasks.filter(t => t.status === 'done');

    renderCol('todo', todo);
    renderCol('prog', prog);
    renderCol('done', done);

    document.getElementById('badge-todo').textContent = todo.length;
    document.getElementById('badge-prog').textContent = prog.length;
    document.getElementById('badge-done').textContent = done.length;

    const allTasks = user.tasks;
    document.getElementById('statTotal').textContent = allTasks.length;
    document.getElementById('statSub').textContent = todo.length + ' To Do · ' + prog.length + ' Progress';
    document.getElementById('statDone').textContent = allTasks.filter(t => t.status === 'done').length;
    const todayDone = allTasks.filter(t => t.status === 'done' && t.completedAt && new Date(t.completedAt).toDateString() === new Date().toDateString()).length;
    document.getElementById('statToday').textContent = todayDone > 0 ? todayDone + ' finalizate azi' : 'Niciuna azi';
    document.getElementById('taskSubtitle').textContent = (todo.length + prog.length) + ' taskuri active';

    document.getElementById('pStatDone').textContent = allTasks.filter(t => t.status === 'done').length;
    document.getElementById('pStatTotal').textContent = allTasks.length;
}

function renderCol(status, tasks) {
    const el = document.getElementById('list-' + status);
    if (tasks.length === 0) {
        el.innerHTML = `<div class="empty"><p>Niciun task</p></div>`;
        return;
    }
    el.innerHTML = tasks.map(t => taskCardHTML(t, status)).join('');
}

const CAT_LABELS = { munca: '💼 Muncă', travel: '✈️ Travel', timp: '🎯 Timp liber', rest: '😌 Rest' };

function taskCardHTML(t, status) {
    const isDone = status === 'done';
    const nextLabel = status === 'todo' ? '→ Progress' : status === 'prog' ? '→ Done' : null;
    const prevLabel = status === 'done' ? '← Progress' : status === 'prog' ? '← To Do' : null;
    const xpEl = isDone
        ? `<span class="xp-tag earned">✓ +10 XP</span>`
        : `<span class="xp-tag">+10 XP</span>`;
    return `
    <div class="task-card ${isDone ? 'done-card' : ''}"
         draggable="true"
         ondragstart="dragStart(event,'${t.id}')"
         id="task-${t.id}">
      <div class="task-name ${isDone ? 'striked' : ''}">${escHtml(t.title)}</div>
      ${t.desc ? `<div class="task-desc">${escHtml(t.desc)}</div>` : ''}
      <div class="task-footer">
        <span class="tag ${t.cat}">${CAT_LABELS[t.cat] || t.cat}</span>
        ${xpEl}
      </div>
      <div class="task-actions">
        ${!isDone ? `<button class="btn btn-secondary btn-sm" onclick="openEditTask('${t.id}')">✏️ Editează</button>` : ''}
        ${nextLabel ? `<button class="btn btn-primary btn-sm" onclick="moveTask('${t.id}','${status === 'todo' ? 'prog' : 'done'}')">${nextLabel}</button>` : ''}
        ${prevLabel ? `<button class="btn btn-secondary btn-sm" onclick="moveTask('${t.id}','${status === 'done' ? 'prog' : 'todo'}')">${prevLabel}</button>` : ''}
        <button class="btn btn-danger btn-sm" onclick="deleteTask('${t.id}')">🗑️</button>
      </div>
    </div>`;
}

// ===================== DRAG & DROP =====================
let dragId = null;

function dragStart(e, id) {
    dragId = id;
    e.dataTransfer.effectAllowed = 'move';
    setTimeout(() => { const el = document.getElementById('task-' + id); if (el) el.classList.add('dragging'); }, 0);
}

function dragOver(e, col) {
    e.preventDefault();
    e.dataTransfer.dropEffect = 'move';
    document.getElementById('col-' + col).classList.add('drag-over');
}

function dragLeave(e) {
    e.currentTarget.classList.remove('drag-over');
}

function drop(e, col) {
    e.preventDefault();
    document.querySelectorAll('.kanban-col').forEach(c => c.classList.remove('drag-over'));
    if (dragId) { moveTask(dragId, col); dragId = null; }
}

// ===================== CALENDAR =====================
let calYear, calMonth, selectedDate = null;

function openCalModal(dateStr) {
    document.getElementById('calTitle').value = '';
    document.getElementById('calDate').value = dateStr || new Date().toISOString().split('T')[0];
    document.getElementById('calColor').value = 'purple';
    openModal('calModal');
    setTimeout(() => document.getElementById('calTitle').focus(), 200);
}

function saveCalEvent() {
    const title = document.getElementById('calTitle').value.trim();
    if (!title) { document.getElementById('calTitle').focus(); return; }
    const user = getCurrentUser();
    user.calEvents = user.calEvents || [];
    user.calEvents.push({
        id: uid(),
        title,
        date: document.getElementById('calDate').value,
        color: document.getElementById('calColor').value
    });
    saveUser(user);
    closeModal('calModal');
    renderCalendar();
    showToast('📅 Eveniment adăugat!', 'success');
}

function deleteCalEvent(id) {
    showConfirm('Șterge eveniment', 'Ești sigur că vrei să ștergi acest eveniment?', () => {
        const user = getCurrentUser();
        user.calEvents = (user.calEvents || []).filter(e => e.id !== id);
        saveUser(user);
        renderCalendar();
        showToast('🗑️ Eveniment șters.', 'success');
    });
}

const MONTHS_RO = ['Ianuarie', 'Februarie', 'Martie', 'Aprilie', 'Mai', 'Iunie', 'Iulie', 'August', 'Septembrie', 'Octombrie', 'Noiembrie', 'Decembrie'];
const DAYS_RO = ['Lu', 'Ma', 'Mi', 'Jo', 'Vi', 'Sa', 'Du'];

function renderCalendar() {
    const now = new Date();
    if (!calYear) { calYear = now.getFullYear(); calMonth = now.getMonth(); }
    document.getElementById('calMonthTitle').textContent = MONTHS_RO[calMonth] + ' ' + calYear;

    const user = getCurrentUser();
    const events = user.calEvents || [];
    const eventsByDate = {};
    events.forEach(e => { if (!eventsByDate[e.date]) eventsByDate[e.date] = []; eventsByDate[e.date].push(e); });

    // Grid header
    const first = new Date(calYear, calMonth, 1);
    const lastDay = new Date(calYear, calMonth + 1, 0).getDate();
    let startDow = first.getDay(); // 0=Sun
    startDow = startDow === 0 ? 6 : startDow - 1; // Mon=0

    let html = DAYS_RO.map(d => `<div class="cal-day-name">${d}</div>`).join('');
    // prev month padding
    const prevLast = new Date(calYear, calMonth, 0).getDate();
    for (let i = startDow - 1; i >= 0; i--) {
        html += `<div class="cal-cell other-month">${prevLast - i}</div>`;
    }
    // current month
    for (let d = 1; d <= lastDay; d++) {
        const dateStr = calYear + '-' + String(calMonth + 1).padStart(2, '0') + '-' + String(d).padStart(2, '0');
        const isToday = now.getFullYear() === calYear && now.getMonth() === calMonth && now.getDate() === d;
        const hasEv = !!eventsByDate[dateStr];
        const isSel = selectedDate === dateStr;
        html += `<div class="cal-cell ${isToday ? 'today' : ''} ${hasEv ? 'has-events' : ''} ${isSel && !isToday ? 'selected' : ''}"
      onclick="selectCalDate('${dateStr}')"
      title="${dateStr}">${d}</div>`;
    }
    // next month padding
    const total = startDow + lastDay;
    const rem = total % 7 === 0 ? 0 : 7 - (total % 7);
    for (let d = 1; d <= rem; d++) html += `<div class="cal-cell other-month">${d}</div>`;

    document.getElementById('calGrid').innerHTML = html;

    // Events list
    let eventsToShow = selectedDate ? (eventsByDate[selectedDate] || []) : events.sort((a, b) => a.date.localeCompare(b.date));
    const titleEl = document.getElementById('calEventsTitle');
    titleEl.textContent = selectedDate ? 'Evenimente pe ' + selectedDate : 'Toate evenimentele (' + events.length + ')';

    const COLOR_MAP = { purple: 'var(--purple)', blue: 'var(--blue)', green: 'var(--green)', amber: 'var(--amber)', red: 'var(--red)' };
    const evHtml = eventsToShow.length
        ? eventsToShow.map(e => `
      <div class="cal-event-item">
        <div class="cal-event-dot" style="background:${COLOR_MAP[e.color] || 'var(--purple)'}"></div>
        <div class="cal-event-title">${escHtml(e.title)}</div>
        <div class="cal-event-date">${e.date}</div>
        <button class="btn btn-danger btn-sm" onclick="deleteCalEvent('${e.id}')">🗑️</button>
      </div>`).join('')
        : `<div class="empty"><p>${selectedDate ? 'Niciun eveniment în această zi.' : 'Niciun eveniment adăugat.'}</p></div>`;

    document.getElementById('calEventsList').innerHTML = evHtml;
}

function selectCalDate(date) {
    selectedDate = selectedDate === date ? null : date;
    renderCalendar();
}

function calPrev() { calMonth--; if (calMonth < 0) { calMonth = 11; calYear--; } selectedDate = null; renderCalendar(); }
function calNext() { calMonth++; if (calMonth > 11) { calMonth = 0; calYear++; } selectedDate = null; renderCalendar(); }

// ===================== NOTES =====================
let editingNoteId = null;

function openAddNote(id) {
    editingNoteId = id || null;
    if (id) {
        const user = getCurrentUser();
        const note = user.notes.find(n => n.id === id);
        document.getElementById('noteContent').value = note ? note.content : '';
        document.getElementById('noteModalTitle').textContent = 'Editează notița';
    } else {
        document.getElementById('noteContent').value = '';
        document.getElementById('noteModalTitle').textContent = 'Notiță nouă';
    }
    openModal('noteModal');
    setTimeout(() => document.getElementById('noteContent').focus(), 200);
}

function saveNote() {
    const content = document.getElementById('noteContent').value.trim();
    if (!content) { document.getElementById('noteContent').focus(); return; }
    const user = getCurrentUser();
    if (editingNoteId) {
        const note = user.notes.find(n => n.id === editingNoteId);
        if (note) { note.content = content; note.updatedAt = Date.now(); }
    } else {
        user.notes.push({ id: uid(), content, createdAt: Date.now(), updatedAt: null });
    }
    saveUser(user);
    closeModal('noteModal');
    renderNotes();
    showToast(editingNoteId ? '✏️ Notiță actualizată!' : '📝 Notiță salvată!', 'success');
}

function deleteNote(id) {
    showConfirm('Șterge notița', 'Ești sigur că vrei să ștergi această notiță?', () => {
        const user = getCurrentUser();
        user.notes = user.notes.filter(n => n.id !== id);
        saveUser(user);
        renderNotes();
        showToast('🗑️ Notiță ștearsă.', 'success');
    });
}

function renderNotes() {
    const user = getCurrentUser();
    const notes = (user.notes || []).slice().reverse();
    const el = document.getElementById('notesGrid');
    if (!notes.length) {
        el.innerHTML = `<div class="empty" style="grid-column:1/-1"><svg viewBox="0 0 24 24"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/></svg><p>Nicio notiță. Adaugă prima!</p></div>`;
        return;
    }
    el.innerHTML = notes.map(n => `
    <div class="note-card" onclick="openAddNote('${n.id}')">
      <div class="note-content">${escHtml(n.content.slice(0, 200))}${n.content.length > 200 ? '...' : ''}</div>
      <div class="note-meta">
        <span>${formatDate(n.updatedAt || n.createdAt)}</span>
        <button class="btn btn-danger btn-sm" onclick="event.stopPropagation();deleteNote('${n.id}')">🗑️</button>
      </div>
    </div>`).join('');
}

// ===================== PROFILE =====================
function renderProfile() {
    const user = getCurrentUser();
    if (!user) return;
    updateXpUI(user);
    document.getElementById('pStatDone').textContent = user.tasks.filter(t => t.status === 'done').length;
    document.getElementById('pStatTotal').textContent = user.tasks.length;
    const theme = document.documentElement.getAttribute('data-theme') || 'light';
    document.getElementById('themeLight').classList.toggle('active', theme === 'light');
    document.getElementById('themeDark').classList.toggle('active', theme === 'dark');
}

// ===================== THEME =====================
function setTheme(t) {
    document.documentElement.setAttribute('data-theme', t);
    document.getElementById('themeLight').classList.toggle('active', t === 'light');
    document.getElementById('themeDark').classList.toggle('active', t === 'dark');
    const user = getCurrentUser();
    if (user) { user.theme = t; saveUser(user); }
}

function toggleTheme() {
    const cur = document.documentElement.getAttribute('data-theme') || 'light';
    setTheme(cur === 'light' ? 'dark' : 'light');
}

// ===================== EXPORT / IMPORT =====================
function exportJSON() {
    const user = getCurrentUser();
    const data = { username: user.username, tasks: user.tasks, notes: user.notes, calEvents: user.calEvents, exportedAt: new Date().toISOString() };
    download('taskmanagement_export.json', JSON.stringify(data, null, 2), 'application/json');
    showToast('📦 Exportat ca JSON!', 'success');
}

function exportTXT() {
    const user = getCurrentUser();
    let txt = `TaskManagement Export — ${new Date().toLocaleDateString('ro-RO')}\nUtilizator: ${user.username}\nNivel: ${user.level} | XP: ${user.xp}\n${'─'.repeat(40)}\n\n`;
    ['todo', 'prog', 'done'].forEach(s => {
        const label = s === 'todo' ? 'TO DO' : s === 'prog' ? 'IN PROGRESS' : 'DONE';
        txt += `[${label}]\n`;
        user.tasks.filter(t => t.status === s).forEach(t => { txt += `  • ${t.title} (${CAT_LABELS[t.cat]})\n`; if (t.desc) txt += `    ${t.desc}\n`; });
        txt += '\n';
    });
    if (user.notes && user.notes.length) {
        txt += `[NOTIȚE]\n`;
        user.notes.forEach(n => { txt += `  • ${n.content.slice(0, 80)}\n`; });
    }
    download('taskmanagement_export.txt', txt, 'text/plain');
    showToast('📄 Exportat ca Text!', 'success');
}

function importJSON(input) {
    const file = input.files[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onload = e => {
        try {
            const data = JSON.parse(e.target.result);
            const user = getCurrentUser();
            if (data.tasks) {
                data.tasks.forEach(t => { if (!user.tasks.find(x => x.id === t.id)) user.tasks.push(t); });
            }
            if (data.notes) {
                data.notes.forEach(n => { if (!user.notes.find(x => x.id === n.id)) user.notes.push(n); });
            }
            if (data.calEvents) {
                user.calEvents = user.calEvents || [];
                data.calEvents.forEach(ev => { if (!user.calEvents.find(x => x.id === ev.id)) user.calEvents.push(ev); });
            }
            saveUser(user);
            renderTasks();
            showToast('⬆️ Import realizat cu succes!', 'success');
        } catch { showToast('❌ Fișier JSON invalid.', 'success'); }
        input.value = '';
    };
    reader.readAsText(file);
}

function download(filename, content, type) {
    const a = document.createElement('a');
    a.href = URL.createObjectURL(new Blob([content], { type }));
    a.download = filename;
    a.click();
}

// ===================== MODALS =====================
function openModal(id) { document.getElementById(id).classList.add('open'); }
function closeModal(id) { document.getElementById(id).classList.remove('open'); }

document.addEventListener('keydown', e => {
    if (e.key === 'Escape') {
        document.querySelectorAll('.modal-backdrop.open').forEach(m => m.classList.remove('open'));
        document.querySelectorAll('.confirm-backdrop.open').forEach(m => m.classList.remove('open'));
    }
});

document.addEventListener('click', e => {
    if (e.target.classList.contains('modal-backdrop')) closeModal(e.target.id);
    if (e.target.classList.contains('confirm-backdrop')) closeConfirm();
});

// Enter key in modals
document.getElementById('taskTitle').addEventListener('keydown', e => { if (e.key === 'Enter') saveTask(); });
document.getElementById('loginPass').addEventListener('keydown', e => { if (e.key === 'Enter') doLogin(); });
document.getElementById('calTitle').addEventListener('keydown', e => { if (e.key === 'Enter') saveCalEvent(); });

// ===================== CONFIRM DIALOG =====================
let confirmCallback = null;
function showConfirm(title, msg, cb) {
    document.getElementById('confirmTitle').textContent = title;
    document.getElementById('confirmMsg').textContent = msg;
    confirmCallback = cb;
    document.getElementById('confirmBackdrop').classList.add('open');
    document.getElementById('confirmOkBtn').onclick = () => { closeConfirm(); cb && cb(); };
}
function closeConfirm() { document.getElementById('confirmBackdrop').classList.remove('open'); confirmCallback = null; }

// ===================== TOAST =====================
function showToast(msg, type = 'success') {
    const c = document.getElementById('toastContainer');
    const t = document.createElement('div');
    t.className = `toast ${type}`;
    t.innerHTML = `<div class="toast-icon">${type === 'level' ? '⭐' : '✓'}</div><span>${msg}</span>`;
    c.appendChild(t);
    document.getElementById('notifDot').style.display = '';
    setTimeout(() => { t.classList.add('leaving'); setTimeout(() => t.remove(), 280); }, 3200);
}

// ===================== UTILS =====================
function escHtml(s) {
    return String(s || '').replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

function formatDate(ts) {
    if (!ts) return '';
    const d = new Date(ts);
    return d.toLocaleDateString('ro-RO', { day: '2-digit', month: 'short', year: 'numeric' });
}

// ===================== BOOT =====================
window.addEventListener('load', () => {
    const username = sessionStorage.getItem('tm_user');
    if (username && getUser(username)) {
        const user = getUser(username);
        setTheme(user.theme || 'light');
        enterApp();
    } else {
        document.getElementById('authScreen').style.display = 'flex';
        document.getElementById('mainApp').style.display = 'none';
    }
});