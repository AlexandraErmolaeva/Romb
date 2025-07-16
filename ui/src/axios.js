import axios from 'axios';

const api = axios.create({
  baseURL: process.env.VUE_APP_API_URL + '/api',  // Укажи свой API URL (например, localhost и порт твоего API)
  timeout: 5000,  // Время ожидания запроса (10 секунд)
});

export default api;