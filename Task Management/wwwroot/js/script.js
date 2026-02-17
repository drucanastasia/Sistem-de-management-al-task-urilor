// ================== Variabile globale ==================
let currentCategory = null;
let tasks = [];
const storageKeyPrefix = "tasks_";
let searchTerm = "";
let userXp = parseInt(localStorage.getItem("userXp")) || 0;
let selectedTaskIds = new Set(); // ✅ AICI — ÎNAINTE DE ORICE FUNCȚIE

// ================== Funcții utilitare ==================
function capitalize(str) {
    return str.charAt(0).toUpperCase() + str.slice(1);
}

function saveTasks() {
    if (!currentCategory) return;
    localStorage.setItem(storageKeyPrefix + currentCategory, JSON.stringify(tasks));
}

// ================== Render ==================
function render() {
    const todoList = document.querySelector("#todo .tasks-list");
    const progressList = document.querySelector("#progress .tasks-list");
    const doneList = document.querySelector("#done .tasks-list");

    if (!todoList || !progressList || !doneList) return;

    todoList.innerHTML = "";
    progressList.innerHTML = "";
    doneList.innerHTML = "";

    tasks.forEach((task, index) => {
        if (searchTerm && !task.text.toLowerCase().includes(searchTerm)) return;

        const el = createTaskElement(task, index);

        if (task.status === "todo") todoList.appendChild(el);
        if (task.status === "progress") progressList.appendChild(el);
        if (task.status === "done") doneList.appendChild(el);
    });

    updateChart();
}

// ================== Task element ==================
function createTaskElement(task, index) {
    const div = document.createElement("div");
    div.className = "task " + task.status;
    div.dataset.id = task.id;

    // Click pe task → toggle selecție SAU mutare status
    div.addEventListener('click', function (e) {
        if (e.target.closest('button')) return; // ignore buttons

        // Dacă e selectat, deselectează
        if (selectedTaskIds.has(task.id)) {
            selectedTaskIds.delete(task.id);
            div.classList.remove('selected');
            updateSelectAllCheckboxes();
            return;
        }

        // Altfel, mută statusul
        const oldStatus = task.status;
        if (task.status === "todo") {
            task.status = "progress";
        } else if (task.status === "progress") {
            task.status = "done";
            addXp(10); // +10 XP când devine Done
        }
        saveTasks();
        render();
    });

    const text = document.createElement("span");
    text.textContent = task.text;

    const editBtn = document.createElement("button");
    editBtn.textContent = "✏️";
    editBtn.onclick = e => {
        e.stopPropagation();
        editTask(index);
    };

    const delBtn = document.createElement("button");
    delBtn.textContent = "🗑️";
    delBtn.onclick = e => {
        e.stopPropagation();
        tasks.splice(index, 1);
        saveTasks();
        render();
        selectedTaskIds.delete(task.id);
    };

    div.appendChild(text);
    div.appendChild(editBtn);
    div.appendChild(delBtn);

    if (selectedTaskIds.has(task.id)) {
        div.classList.add('selected');
    }

    return div;
}

// ================== Task logic ==================
function editTask(index) {
    const text = prompt("Editează task-ul", tasks[index].text);
    if (!text) return;
    tasks[index].text = text.trim();
    saveTasks();
    render();
}

function addTask() {
    const input = document.getElementById("taskInput");
    if (!input || !currentCategory) return;

    const text = input.value.trim();
    if (text === "") return;

    tasks.push({
        id: Date.now(),
        text: text,
        status: "todo"
    });

    input.value = "";
    saveTasks();
    render();
}

// ================== Chart ==================
let chartInstance = null;

function initChart() {
    const canvas = document.getElementById("chart");
    if (!canvas) return;

    chartInstance = new Chart(canvas, {
        type: "doughnut",
        data: {
            labels: ["To Do", "In Progress", "Done"],
            datasets: [{
                data: [0, 0, 0],
                backgroundColor: ["#ffcc00", "#36a2eb", "#4caf50"]
            }]
        },
        options: {
            plugins: { legend: { display: false } },
            cutout: "70%"
        }
    });
}

function updateChart() {
    if (!chartInstance) return;

    const todo = tasks.filter(t => t.status === "todo").length;
    const progress = tasks.filter(t => t.status === "progress").length;
    const done = tasks.filter(t => t.status === "done").length;

    chartInstance.data.datasets[0].data = [todo, progress, done];
    chartInstance.update();

    document.getElementById("percent").textContent =
        tasks.length ? Math.round(done / tasks.length * 100) + "%" : "0%";
}

// ================== XP SYSTEM ==================
function updateXpDisplay() {
    const xpEl = document.getElementById("userXp");
    const fillEl = document.getElementById("xpFill");
    if (!xpEl || !fillEl) return;

    xpEl.textContent = userXp;
    const level = Math.floor(userXp / 100);
    const xpInLevel = userXp % 100;
    const percent = Math.min(100, xpInLevel / 100 * 100);

    setTimeout(() => {
        fillEl.style.width = percent + "%";
    }, 10);
}

function addXp(amount) {
    userXp += amount;
    localStorage.setItem("userXp", userXp.toString());
    updateXpDisplay();
    checkUnlocks();
}

function checkUnlocks() {
    if (userXp >= 50 && !localStorage.getItem("unlocked_themes")) {
        alert("🔓 Ai deblocat Teme Personalizate!");
        localStorage.setItem("unlocked_themes", "true");
    }
    if (userXp >= 200 && !localStorage.getItem("unlocked_calendar")) {
        alert("🔓 Ai deblocat Calendar Avansat!");
        localStorage.setItem("unlocked_calendar", "true");
    }
}

// ================== Selectare masivă ==================
function updateSelectAllCheckboxes() {
    ['todo', 'progress', 'done'].forEach(col => {
        const tasksInCol = tasks.filter(t => t.status === col);
        const selectedInCol = tasksInCol.filter(t => selectedTaskIds.has(t.id));
        const checkbox = document.querySelector(`.select-all[data-column="${col}"]`);

        if (checkbox) {
            if (selectedInCol.length === 0) {
                checkbox.checked = false;
                checkbox.indeterminate = false;
            } else if (selectedInCol.length === tasksInCol.length) {
                checkbox.checked = true;
                checkbox.indeterminate = false;
            } else {
                checkbox.checked = false;
                checkbox.indeterminate = true;
            }
        }
    });
}

document.querySelectorAll('.select-all').forEach(checkbox => {
    checkbox.addEventListener('change', function () {
        const column = this.dataset.column;
        const tasksInColumn = document.querySelectorAll(`#${column} .task`);

        tasksInColumn.forEach(taskEl => {
            const taskId = taskEl.dataset.id;
            if (this.checked) {
                selectedTaskIds.add(taskId);
                taskEl.classList.add('selected');
            } else {
                selectedTaskIds.delete(taskId);
                taskEl.classList.remove('selected');
            }
        });
        updateSelectAllCheckboxes();
    });
});

function deleteSelectedTasks() {
    if (selectedTaskIds.size === 0) return;
    if (!confirm(`Ștergi ${selectedTaskIds.size} task-uri?`)) return;

    tasks = tasks.filter(task => !selectedTaskIds.has(task.id));
    selectedTaskIds.clear();
    saveTasks();
    render();
}

function initDeleteButton() {
    if (document.getElementById('deleteSelectedBtn')) return;
    
    const deleteBtn = document.createElement('button');
    deleteBtn.id = 'deleteSelectedBtn';
    deleteBtn.className = 'delete-selected-btn';
    deleteBtn.onclick = deleteSelectedTasks;
    
    const exportImport = document.querySelector('.export-import');
    if (exportImport) {
        exportImport.parentNode.insertBefore(deleteBtn, exportImport);
    }
}

// ================== Navigare ==================
function setupNavigation() {
    const menu = document.getElementById("menuScreen");
    const taskScreen = document.getElementById("taskScreen");
    const pageTitle = document.getElementById("pageTitle");
    const quoteTask = document.getElementById("quoteTask");

    const quotes = {
        university: "Cunoașterea este putere, organizeaz-o.",
        work: "Productivitatea începe cu planificarea.",
        travel: "Fiecare drum începe cu un pas.",
        personal: "Planifică-ți ziua."
    };

    document.querySelectorAll(".cat-btn").forEach(btn => {
        btn.addEventListener("click", () => {
            currentCategory = btn.dataset.cat;
            tasks = JSON.parse(localStorage.getItem(storageKeyPrefix + currentCategory)) || [];

            pageTitle.textContent = "Task Manager " + capitalize(currentCategory);
            quoteTask.textContent = quotes[currentCategory];

            menu.style.display = "none";
            taskScreen.style.display = "block";

            setTimeout(() => {
                const input = document.getElementById("taskInput");
                if (input) input.focus();
            }, 100);

            render();
            updateXpDisplay();
        });
    });
}

// ================== Navigare cu săgetile browserului ==================
window.addEventListener("popstate", function (e) {
    const menu = document.getElementById("menuScreen");
    const taskScreen = document.getElementById("taskScreen");
    const pageTitle = document.getElementById("pageTitle");
    const quoteTask = document.getElementById("quoteTask");

    const quotes = {
        university: "Cunoașterea este putere, organizeaz-o.",
        work: "Productivitatea începe cu planificarea.",
        travel: "Fiecare drum începe cu un pas.",
        personal: "Planifică-ți ziua."
    };

    if (e.state && e.state.view === "task") {
        currentCategory = e.state.cat;
        tasks = JSON.parse(localStorage.getItem(storageKeyPrefix + currentCategory)) || [];

        menu.style.display = "none";
        taskScreen.style.display = "block";
        pageTitle.textContent = "Task Manager " + capitalize(currentCategory);
        quoteTask.textContent = quotes[currentCategory];
        render();
        updateXpDisplay();
    } else {
        menu.style.display = "block";
        taskScreen.style.display = "none";
        currentCategory = null;
        tasks = [];
        selectedTaskIds.clear();
    }
});

// ================== Login / Profil ==================
function updateAuthButton() {
    const authBtn = document.getElementById("authButton");
    if (!authBtn) return;

    const isLoggedIn = localStorage.getItem("isLoggedIn") === "true";

    if (isLoggedIn) {
        authBtn.innerHTML = "👤";
        authBtn.title = "Profilul meu";
        authBtn.onclick = () => {
            alert("Deschide pagina de profil (implementează după)");
        };
    } else {
        authBtn.innerHTML = "🔒";
        authBtn.title = "Login";
        authBtn.onclick = () => {
            if (confirm("Vrei să te loghezi?")) {
                localStorage.setItem("isLoggedIn", "true");
                updateAuthButton();
                alert("Logat cu succes!");
            }
        };
    }
}

// ================== EXPORT ==================
function exportTasks() {
    if (!currentCategory || tasks.length === 0) {
        alert("Niciun task de exportat!");
        return;
    }

    const format = document.getElementById('exportFormat').value;
    const data = JSON.stringify(tasks, null, 2);
    const filename = `tasks_${currentCategory}.${format}`;

    switch (format) {
        case 'json':
            downloadFile(filename, data, 'application/json');
            break;
        case 'csv':
            const csv = convertToCSV(tasks);
            downloadFile(filename, csv, 'text/csv');
            break;
        case 'txt':
            const txt = tasks.map(t => `${t.status}: ${t.text}`).join('\n');
            downloadFile(filename, txt, 'text/plain');
            break;
    }
}

// ================== IMPORT ==================
function importTasks() {
    const format = document.getElementById('importFormat').value;

    const input = document.createElement('input');
    input.type = 'file';
    input.accept = `.${format}`;

    input.onchange = e => {
        const file = e.target.files[0];
        if (!file) return;

        const reader = new FileReader();
        reader.onload = event => {
            try {
                let importedTasks = [];

                switch (format) {
                    case 'json':
                        importedTasks = JSON.parse(event.target.result);
                        break;
                    case 'csv':
                        importedTasks = parseCSV(event.target.result);
                        break;
                    case 'txt':
                        importedTasks = parseTXT(event.target.result);
                        break;
                }

                if (Array.isArray(importedTasks)) {
                    tasks = importedTasks;
                    saveTasks();
                    render();
                    alert(`Importat ${importedTasks.length} task-uri!`);
                }
            } catch (error) {
                alert('Eroare la import: ' + error.message);
            }
        };
        reader.readAsText(file);
    };

    input.click();
}

// ================== UTILITARE ==================
function downloadFile(filename, content, type) {
    const blob = new Blob([content], { type });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
}

function convertToCSV(tasks) {
    const header = 'ID,Text,Status\n';
    const rows = tasks.map(t => `"${t.id}","${t.text.replace(/"/g, '""')}","${t.status}"`).join('\n');
    return header + rows;
}

function parseCSV(csv) {
    const lines = csv.trim().split('\n');
    const tasks = [];

    for (let i = 1; i < lines.length; i++) {
        const line = lines[i];
        const match = line.match(/^"([^"]*)","([^"]*)","([^"]*)"$/);
        if (match) {
            tasks.push({
                id: parseInt(match[1]) || Date.now(),
                text: match[2],
                status: ['todo', 'progress', 'done'].includes(match[3]) ? match[3] : 'todo'
            });
        }
    }
    return tasks;
}

function parseTXT(txt) {
    return txt.split('\n').map(line => {
        const match = line.match(/^(todo|progress|done):\s*(.*)$/i);
        return {
            id: Date.now(),
            text: match ? match[2] : line,
            status: match ? match[1].toLowerCase() : 'todo'
        };
    }).filter(t => t.text.trim());
}

// ================== Calendar zilnic ==================
let currentDate = new Date();

function formatDate(date) {
    const options = { day: 'numeric', month: 'long', year: 'numeric' };
    return date.toLocaleDateString('ro-RO', options);
}

function loadTasksForDate(dateStr) {
    return JSON.parse(localStorage.getItem(`tasks_${dateStr}`)) || [];
}

function saveTasksForDate(dateStr, tasks) {
    localStorage.setItem(`tasks_${dateStr}`, JSON.stringify(tasks));
}

function renderCalendar() {
    document.getElementById("currentDate").textContent = formatDate(currentDate);

    const dateStr = currentDate.toISOString().split('T')[0];
    const tasks = loadTasksForDate(dateStr);

    const todo = tasks.filter(t => t.status === "todo");
    const progress = tasks.filter(t => t.status === "progress");
    const done = tasks.filter(t => t.status === "done");

    document.getElementById("todoList").innerHTML =
        todo.length ? todo.map(t => `<div class="task">${t.text}</div>`).join("") : "<em>Niciun task</em>";
    document.getElementById("progressList").innerHTML =
        progress.length ? progress.map(t => `<div class="task">${t.text}</div>`).join("") : "<em>Niciun task</em>";
    document.getElementById("doneList").innerHTML =
        done.length ? done.map(t => `<div class="task">${t.text}</div>`).join("") : "<em>Niciun task</em>";
}

document.getElementById("prevDay")?.addEventListener('click', () => {
    currentDate.setDate(currentDate.getDate() - 1);
    renderCalendar();
});
document.getElementById("nextDay")?.addEventListener('click', () => {
    currentDate.setDate(currentDate.getDate() + 1);
    renderCalendar();
});

// ================== EVENT LISTENERS ==================
document.addEventListener("DOMContentLoaded", () => {
    // Inițializare
    document.getElementById("menuScreen").style.display = "block";
    document.getElementById("taskScreen").style.display = "none";

    initChart();
    setupNavigation();
    updateAuthButton();
    updateXpDisplay();
    initDeleteButton();

    // Buton Adaugă
    document.getElementById("addBtn")?.addEventListener("click", addTask);

    // Input Enter
    document.getElementById("taskInput")?.addEventListener("keypress", e => {
        if (e.key === "Enter") addTask();
    });

    // Căutare
    document.getElementById("searchInput")?.addEventListener("input", e => {
        searchTerm = e.target.value.toLowerCase();
        render();
    });

    // Notițe
    const notes = document.getElementById("personalNotes");
    if (notes) {
        notes.value = localStorage.getItem("personalNotes") || "";
        notes.addEventListener("input", () => {
            localStorage.setItem("personalNotes", notes.value);
        });
    }

    // Export/Import
    document.getElementById('exportBtn')?.addEventListener('click', exportTasks);
    document.getElementById('importBtn')?.addEventListener('click', importTasks);

    // Calendar
    renderCalendar();
});

// === Selectare masivă — FIX pentru cutia ✓ ===
document.querySelectorAll('.select-all').forEach(checkbox => {
    checkbox.addEventListener('change', function () {
        const col = this.dataset.column; // "todo", "progress", "done"

        // 1. Actualizează selectedTaskIds din array-ul tasks
        if (this.checked) {
            tasks.filter(t => t.status === col).forEach(t => selectedTaskIds.add(t.id));
        } else {
            tasks.filter(t => t.status === col).forEach(t => selectedTaskIds.delete(t.id));
        }

        // 2. Marchează/DEMARCHHEAZĂ în UI
        document.querySelectorAll(`#${col} .task`).forEach(el => {
            const id = el.dataset.id;
            if (this.checked && selectedTaskIds.has(id)) {
                el.classList.add('selected');
            } else {
                el.classList.remove('selected');
            }
        });

        updateSelectAllCheckboxes();
    });
});
// === Popup plată ===
const paymentPopup = document.getElementById('paymentPopup');
const closePopupBtn = document.querySelector('.close-popup');
const payBtns = document.querySelectorAll('.pay-btn');

// Deschide popup la click pe butonul Calendar
document.querySelector('button[onclick*="/Calendar"]')?.addEventListener('click', function (e) {
    e.preventDefault();
    paymentPopup.style.display = 'flex';
});

// Închide popup
closePopupBtn.addEventListener('click', () => {
    paymentPopup.style.display = 'none';
});

// Plată efectuată → deblochează calendarul
payBtns.forEach(btn => {
    btn.addEventListener('click', function () {
        const plan = this.dataset.plan;

        // Simulează plată (în realitate, aici ai API call)
        localStorage.setItem('isPremium', 'true');
        localStorage.setItem('premiumPlan', plan);

        alert(`✅ Plata confirmată! Contul tău este acum PREMIUM.`);
        paymentPopup.style.display = 'none';

        // Redirect către calendar (unde vei avea funcționalități premium)
        window.location.href = '/Calendar';
    });
});
document.getElementById('calendarBtn')?.addEventListener('click', function (e) {
    e.preventDefault();

    // Dacă e premium → mergi direct la calendar
    if (localStorage.getItem('isPremium') === 'true') {
        window.location.href = '/Calendar';
        return;
    }

    // Altfel → deschide popup-ul de plată
    document.getElementById('paymentPopup').style.display = 'flex';
});