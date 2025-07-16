<template>
  <div>
    <h1>Запланированные мероприятия</h1>
    <button @click="$router.push('/add/plannedevent')">Запланировать</button>
    <button @click="$router.push('/actualevents')">Выполненные мероприятия</button>
    <table>
      <thead>
        <tr>
          <th>ID</th>
          <th>Код цели</th>
          <th>Наименование</th>
          <th>Общий бюджет</th>
          <th>Планируемый уровень софинансирования</th>
          <th>Планируемый местный бюджет</th>
          <th>Планируемый региональный бюджет</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="event in sortedPlannedEvents" :key="event.id">
          <td>{{ event.id }}</td>
          <td>{{ event.targetCode }}</td>
          <td>{{ event.name }}</td>
          <td>{{ event.totalBudget }}</td>
          <td>{{ event.plannedCofinanceRate }}%</td>
          <td>{{ event.plannedLocalBudget }}</td>
          <td>{{ event.plannedRegionalBudget }}</td>
        </tr>
      </tbody>
    </table>
    <div v-if="loading">Загрузка...</div>
    <div v-if="error" class="error">{{ error }}</div>
  </div>
</template>

<script>
import axios from '@/axios';

export default {
  data() {
    return {
      plannedEvents: [],
      loading: false,
      error: null
    };
  },
  methods: {
    async fetchPlannedEvents() {
      this.loading = true;
      this.error = null;
      try {
        const response = await axios.get(`/PlannedEvent`);
        this.plannedEvents = response.data;
      } catch (err) {
        this.error = 'Не удалось загрузить мероприятия: ' + err.message;
      } finally {
        this.loading = false;
      }
    },
  },
  created() {
    this.fetchPlannedEvents();
  },
  computed: {
    sortedPlannedEvents() {
      return [...this.plannedEvents].sort((a, b) => a.id - b.id);
    }
  }
};
</script>

<style scoped>
/* Общий стиль для страницы */
div {
  padding: 20px;
  font-family: Arial, sans-serif;
}

/* Заголовок */
h1 {
  margin-bottom: 20px;
  color: #333;
}

/* Кнопка */
button {
  background-color: #448ca5;  /* Зелёный цвет */
  color: white;
  padding: 10px 20px;
  margin-bottom: 20px;
  margin: 10px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 16px;
}

button:hover {
  background-color: #4cc0ba;
}

/* Таблица */
table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 10px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

th, td {
  padding: 12px 15px;
  text-align: center;
  border-bottom: 1px solid #ddd;
}

th {
  background-color: #f2f2f2;
  color: #386554;
  font-weight: bold;
}

tr:nth-child(even) {
  background-color: #f9f9f9;
}

tr:hover {
  background-color: #f1f1f1;
  transition: background-color 0.3s ease;
}

/* Стиль для ошибки */
.error {
  color: red;
  margin-top: 20px;
  font-weight: bold;
}
</style>
