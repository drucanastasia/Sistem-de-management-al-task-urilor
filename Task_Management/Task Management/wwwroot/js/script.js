let currentCategory = null;
let tasks = [];
const storageKeyPrefix = "tasks_";
let searchTerm = ""; // 🔍 pentru căutare

// === DOM elements ===
const menuScreen = document.getElementById("menuScreen");
const taskScreen = document.getElementById("taskScreen");
const catButtons = document.querySelectorAll(".cat-btn");
const backBtn = document.getElementById("backBtn");
const pageTitle = document.getElementById("pageTitle");
const quoteTask = document.getElementById("quoteTask");
const taskInput = document.getElementById("taskInput");
const addBtn = document.getElementById("addBtn");
const searchInput = document.getElementById("searchInput"); // 🔍
const todoList = document.querySelector("#todo .tasks-list");
const progressList = document.querySelector("#progress .tasks-list");
const doneList = document.querySelector("#done .tasks-list");
const chartCanvas = document.getElementById("chart");
const percentEl = document.getElementById("percent");
const statsSummary = document.getElementById("statsSummary"); // 📊
const notesTextarea = document.getElementById("personalNotes");
const exportBtn = document.getElementById("exportBtn"); // 💾
const importBtn = document.getElementById("importBtn"); // 📥

// === Citate per categorie ===
const categoryQuotes = {
  university: "“Cunoașterea este putere — organizeaz-o!”",
  work: "“Productivitatea începe cu planificarea clară.”",
  travel: "“O călătorie de mii de mile începe cu un singur pas.”",
  personal: "“Planifică-ți ziua și ziua te va răsplăti”"
};

// === Inițializează graficul ===
let chart;
function initChart() {
  const ctx = chartCanvas.getContext("2d");
  chart = new Chart(ctx, {
    type: 'doughnut',
    data: {
      labels: ['To Do', 'In Progress', 'Done'],
      datasets: [{
        data: [0, 0, 0],
        backgroundColor: ['#ffcc00', '#36a2eb', '#4caf50'],
        borderWidth: 4,
        borderColor: '#ffffff'
      }]
    },
    options: {
      responsive: false,
      plugins: { legend: { display: false } },
      cutout: '70%'
    }
  });
}
initChart();

// === Schimbă ecranul → taskScreen ===
catButtons.forEach(btn => {
  btn.addEventListener("click", () => {
    currentCategory = btn.dataset.cat;
    const key = `${storageKeyPrefix}${currentCategory}`;
    tasks = JSON.parse(localStorage.getItem(key)) || [];

    pageTitle.textContent = `Task Manager — ${capitalize(currentCategory)}`;
    quoteTask.textContent = categoryQuotes[currentCategory] || categoryQuotes.personal;

    menuScreen.style.display = "none";
    taskScreen.style.display = "block";

    render();
  });
});

// === Înapoi la meniu ===
backBtn.addEventListener("click", () => {
  taskScreen.style.display = "none";
  menuScreen.style.display = "block";
  currentCategory = null;
});

// === Căutare ===
if (searchInput) {
  searchInput.addEventListener("input", (e) => {
    searchTerm = e.target.value.toLowerCase();
    render();
  });
}

// === Export ===
if (exportBtn) {
  exportBtn.addEventListener("click", () => {
    const data = {
      tasks_university: localStorage.getItem("tasks_university"),
      tasks_work: localStorage.getItem("tasks_work"),
      tasks_travel: localStorage.getItem("tasks_travel"),
      tasks_personal: localStorage.getItem("tasks_personal"),
      personalNotes: localStorage.getItem("personalNotes")
    };
    const blob = new Blob([JSON.stringify(data, null, 2)], {type: 'application/json'});
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = "task-manager-backup.json";
    a.click();
    URL.revokeObjectURL(url);
  });
}

// === Import ===
if (importBtn) {
  importBtn.addEventListener("click", () => {
    const input = document.createElement("input");
    input.type = "file";
    input.accept = ".json";
    input.onchange = e => {
      const file = e.target.files[0];
      if (!file) return;
      const reader = new FileReader();
      reader.onload = () => {
        try {
          const data = JSON.parse(reader.result);
          Object.keys(data).forEach(key => {
            if (data[key] !== null) {
              localStorage.setItem(key, data[key]);
            }
          });
          alert("✅ Datele au fost importate cu succes!");
          location.reload();
        } catch (err) {
          alert("❌ Fișier invalid!");
        }
      };
      reader.readAsText(file);
    };
    input.click();
  });
}

// === Funcții ajutătoare ===
function capitalize(str) {
  return str.charAt(0).toUpperCase() + str.slice(1);
}

function saveTasks() {
  if (!currentCategory) return;
  const key = `${storageKeyPrefix}${currentCategory}`;
  localStorage.setItem(key, JSON.stringify(tasks));
}

function render() {
  if (!todoList || !progressList || !doneList) {
    console.error("❌ Lipsesc elementele .tasks-list în HTML!");
    return;
  }

  todoList.innerHTML = "";
  progressList.innerHTML = "";
  doneList.innerHTML = "";

  tasks.forEach((task, index) => {
    // 🔍 Filtrare căutare
    if (searchTerm && !task.text.toLowerCase().includes(searchTerm)) {
      return;
    }

    const el = createTaskElement(task, index);
    if (task.status === "todo") {
      todoList.appendChild(el);
    } else if (task.status === "progress") {
      progressList.appendChild(el);
    } else if (task.status === "done") {
      doneList.appendChild(el);
    }
  });

  updateChart();
}

function createTaskElement(task, index) {
  const div = document.createElement("div");
  div.className = `task ${task.status}`;
  div.dataset.index = index;

  const textSpan = document.createElement("span");
  textSpan.className = "task-text";
  textSpan.textContent = task.text;

  const editBtn = document.createElement("button");
  editBtn.className = "task-btn edit";
  editBtn.innerHTML = "✏️";
  editBtn.title = "Editează";
  editBtn.onclick = (e) => {
    e.stopPropagation();
    editTask(index);
  };

  const delBtn = document.createElement("button");
  delBtn.className = "task-btn delete";
  delBtn.innerHTML = "🗑️";
  delBtn.title = "Șterge";
  delBtn.onclick = (e) => {
    e.stopPropagation();
    tasks.splice(index, 1);
    saveTasks();
    render();
  };

  const actions = document.createElement("div");
  actions.className = "task-actions";
  actions.appendChild(editBtn);
  actions.appendChild(delBtn);

  div.appendChild(textSpan);
  div.appendChild(actions);

  div.addEventListener("click", () => {
    if (task.status === "todo") {
      task.status = "progress";
    } else if (task.status === "progress") {
      task.status = "done";
    }
    saveTasks();
    render();
  });

  return div;
}

function editTask(index) {
  const task = tasks[index];
  const newText = prompt("Editează task-ul:", task.text);
  if (newText !== null && newText.trim() !== "") {
    task.text = newText.trim();
    saveTasks();
    render();
  }
}

// === Adăugare task ===
addBtn.addEventListener("click", addTask);
taskInput.addEventListener("keypress", e => e.key === "Enter" && addTask());

function addTask() {
  const text = taskInput.value.trim();
  if (!text || !currentCategory) return;

  tasks.push({ id: Date.now(), text, status: "todo" });
  taskInput.value = "";
  saveTasks();
  render();
}

// === Actualizează graficul + statistici ===
function updateChart() {
  const todo = tasks.filter(t => t.status === "todo").length;
  const progress = tasks.filter(t => t.status === "progress").length;
  const done = tasks.filter(t => t.status === "done").length;
  const total = todo + progress + done;
  const percent = total ? Math.round((done / total) * 100) : 0;

  percentEl.textContent = `${percent}%`;
  if (chart) {
    chart.data.datasets[0].data = [todo, progress, done];
    chart.update();
  }

  // 📊 Statistici simple
  if (statsSummary) {
    statsSummary.textContent = `Total: ${total} • Finalizate: ${done}`;
  }
}

// === Notițe personale ===
notesTextarea.value = localStorage.getItem("personalNotes") || "";
notesTextarea.addEventListener("input", () => {
  localStorage.setItem("personalNotes", notesTextarea.value);
});

// === Initial — afișează meniul ===
document.addEventListener("DOMContentLoaded", () => {
  const quotes = [
    "“Organizează-ți ziua și ziua te va răsplăti”",
    "“Fă astăzi ceea ce alții nu vor și mâine vei avea ce alții nu pot”"
  ];
  document.getElementById("quote").textContent = quotes[Math.floor(Math.random() * quotes.length)];
});