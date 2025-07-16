<template>
    <div>
      <h1>Выполненные мероприятий</h1>
      <button @click="$router.push('/add/actualevent')">Добавить</button>
      <button @click="$router.push('/home')">Назад</button>
      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Код цели</th>
            <th>Сумма выполненных работ</th>
            <th>Фактический уровень софинансирования</th>
            <th>Фактическаий местный бюджет</th>
            <th>Фактический областной бюджет </th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="event in sortedActualEvents" :key="event.id">
            <td>{{ event.plannedEventId }}</td>
            <td>{{ event.targetCode }}</td>
            <td>{{ event.completedWorksBudget }}</td>
            <td>{{ event.actualCofinanceRate }}%</td>
            <td>{{ event.actualLocalBudget }}</td>
            <td>{{ event.actualRegionalBudget }}</td>
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
      actualEvents: [],
      loading: false,
      error: null
    };
  },
  methods: {
    async fetchActualEvents() {
      this.loading = true;
      this.error = null;
      try {
        const response = await axios.get(`/ActualEvent`);
        this.actualEvents = response.data;
      } catch (err) {
        this.error = 'Не удалось загрузить мероприятия: ' + err.message;
      } finally {
        this.loading = false;
      }
    },
  },
  created() {
    this.fetchActualEvents();
  },
  computed: {
    sortedActualEvents() {
      return [...this.actualEvents].sort((a, b) => a.id - b.id);
    }
  },
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
  background-color: #7faf4c;  /* Зелёный цвет */
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
  background-color: #71a045;
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
  color: #40564e;
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
