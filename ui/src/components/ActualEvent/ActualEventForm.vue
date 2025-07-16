<template>
    <div class="form-container">
      <h1>Добавить выполненное мероприятие</h1>
  
      <form @submit.prevent="submitForm" class="actualEvent-form">
        <label for="plannedEventId">ID запланированного мероприятия</label>
        <input type="number" id="plannedEventId" v-model="actualEvent.plannedEventId" required />
  
        <label for="completedWorksBudget">Сумма выполненных работ</label>
        <input type="text" id="completedWorksBudget" v-model="actualEvent.completedWorksBudget" required />
  
        <button type="submit">Добавить</button>
      </form>
  
      <button @click="$router.push('/actualevents')">Назад</button>
    </div>
  </template>
  
    <script>
    import axios from '@/axios';
    
    export default {
      data() {
        return {
          actualEvent: {
            plannedEventId: 0,
            completedWorksBudget: 0
          }
        };
      },
      methods: {
          async submitForm() {
        this.actualEvent.completedWorksBudget = this.formatDecimal(this.actualEvent.completedWorksBudget);
  
        try {
          // Отправляем данные на сервер
          await axios.post(`/ActualEvent`, this.actualEvent);
          this.$router.push('/actualevents'); // После успешного добавления переходим обратно в список
        } catch (error) {
          alert('Ошибка при добавлении мероприятия: ' + error.message);
        }
      },
      formatDecimal(value) {
        // Проверяем и преобразуем в число с плавающей точкой и точностью до 15 знаков
        const num = parseFloat(value);
        if (isNaN(num)) return '0'; // если значение не числовое, то возвращаем 0
        return num.toFixed(15); // Округляем до 15 знаков после запятой
      }
    }
  };
  </script>
    
    <style scoped>
    /* Контейнер для формы */
    .form-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      height: 100vh;
      padding: 20px;
      background-color: #f4f4f9; /* Легкий фон для контраста */
    }
    
    /* Заголовок */
    h1 {
      font-size: 24px;
      margin-bottom: 20px;
      color: #333;
      text-align: center;
    }
    
    /* Основная форма */
    .actualEvent-form {
      display: flex;
      flex-direction: column;
      width: 100%;
      max-width: 400px; /* Ограничиваем ширину формы */
      padding: 20px;
      background-color: white;
      border-radius: 8px;
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    }
    
    /* Метки */
    label {
      font-size: 14px;
      color: #555;
      margin-bottom: 8px;
      margin-top: 10px;
      text-align: left;
    }
    
    /* Инпуты */
    input {
      padding: 10px;
      margin-bottom: 15px;
      border: 1px solid #ccc;
      border-radius: 5px;
      font-size: 14px;
      outline: none;
    }
    
    input:focus {
      border-color: #4caf75;
    }
    
    /* Кнопка отправки формы */
    button {
      padding: 12px 20px;
      background-color: #4caf7d;
      color: white;
      border: none;
      border-radius: 5px;
      font-size: 16px;
      cursor: pointer;
      transition: background-color 0.3s ease;
      margin-top: 20px;
    }
    
    button:hover {
      background-color: #45a049;
    }
    
    /* Кнопка Назад */
    button:nth-of-type(2) {
      background-color: #6c757d;
      margin-top: 10px;
    }
    
    button:nth-of-type(2):hover {
      background-color: #5a6268;
    }
    
    /* Отступы для кнопки Назад */
    button:last-of-type {
      margin-top: 20px;
    }
    </style>
    
    