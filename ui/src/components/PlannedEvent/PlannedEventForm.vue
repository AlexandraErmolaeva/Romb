<template>
  <div class="form-container">
    <h1>Вычисления</h1>

    <form @submit.prevent="calculate" class="calculator-form">
      
      <label for="powBase">Основание</label>
      <input id="powBase" v-model="pow.powBase" type="number" step="any" required />

      <label for="powExponent">Степень</label>
      <input id="powExponent" v-model="pow.powExponent" type="number" step="any" required />

      <button type="submit">Вычислить</button>
    </form>

    <button @click="$router.push('/home')">Назад</button>

    <div v-if="result !== null">
      <h2>Результат: {{ result }}</h2>
    </div>

    <div v-if="errorMessage" class="error">
      <p>{{ errorMessage }}</p>
    </div>
  </div>
</template>

<script>
import api from "../api";

export default {
  data() {
    return {
      pow: {
        powBase: 0,  // Основание
        powExponent: 0,  // Степень
      },
      result: null,
      errorMessage: "",
    };
  },
  methods: {
    async calculate() {
  this.result = null;
  this.errorMessage = "";

  let body = {};

  // Для операций с несколькими числами
  if (this.operation !== 'root' && this.operation !== 'pow-from-base' && this.operation !== 'pow') {
    if (!this.numbers) {
      this.errorMessage = "Введите корректное выражение";
      return;
    }

    const numbers = this.numbers.split(',').map(num => parseFloat(num.trim()));

    if (numbers.some(isNaN)) {
      this.errorMessage = "Некорректные значения в выражении";
      return;
    }

    body.numbers = numbers; 
  }

  // Для операции извлечения корня
  if (this.operation === 'root') {
    // Проверяем, что значения корректно преобразуются в числа
    const rootBase = parseFloat(this.rootBase);
    const rootExponent = parseFloat(this.rootExponent);

    if (isNaN(rootBase) || isNaN(rootExponent)) {
      this.errorMessage = "Введите корректные значения для основания и степени корня";
      return;
    }

    body.rootBase = rootBase; // Основание
    body.rootExponent = rootExponent; // Степень корня
  }

  // Для операции возведения в степень
  if (this.operation === 'pow') {
    // Проверяем, что значения корректно преобразуются в числа
    const powBase = parseFloat(this.powBase);
    const powExponent = parseFloat(this.powExponent);

    if (isNaN(powBase) || isNaN(powExponent)) {
      this.errorMessage = "Введите корректные значения для числа и степени";
      return;
    }

    body.powBase = powBase; // Число
    body.powExponent = powExponent; // Степень
  }

  // Для операции нахождения степени по основанию
  if (this.operation === 'pow-from-base') {
    // Проверяем, что значения корректно преобразуются в числа
    const powBaseFromBase = parseFloat(this.powBaseFromBase);
    const powNumber = parseFloat(this.powNumber);

    if (isNaN(powBaseFromBase) || isNaN(powNumber)) {
      this.errorMessage = "Введите корректные значения для основания и числа";
      return;
    }

    body.base = powBaseFromBase;
    body.number = powNumber;
  }

  // Отправка запроса на сервер
  try {
    const response = await api.post(`/calculator/${this.operation}`, body);
    this.result = response.data.result;
  } catch (error) {
    this.errorMessage = "Ошибка при вычислении";
  }
}
  }
}

</script>

<style scoped>
/* Стиль формы */
.form-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100vh;
  padding: 20px;
  background-color: #f4f4f9;
}

/* Заголовок */
h1 {
  font-size: 24px;
  margin-bottom: 20px;
  color: #333;
  text-align: center;
}

/* Форма */
.calculator-form {
  display: flex;
  flex-direction: column;
  width: 100%;
  max-width: 400px;
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
  border-color: #3a8596;
}

/* Кнопка отправки формы */
button {
  padding: 12px 20px;
  background-color: #3a8c98;
  color: white;
  border: none;
  border-radius: 5px;
  font-size: 16px;
  cursor: pointer;
  transition: background-color 0.3s ease;
  margin-top: 20px;
}

button:hover {
  background-color: #4cc0ba;
}

/* Кнопка Назад */
button:nth-of-type(2) {
  background-color: #6c757d;
  margin-top: 10px;
}

button:nth-of-type(2):hover {
  background-color: #5a6268;
}

/* Сообщение об ошибке */
.error {
  color: red;
  font-size: 16px;
  margin-top: 20px;
}
</style>
