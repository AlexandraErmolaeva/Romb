import { createRouter, createWebHistory } from 'vue-router';
import PlannedEventList from '@/components/PlannedEvent/PlannedEventList.vue';
import PlannedEventForm from '@/components/PlannedEvent/PlannedEventForm.vue';
import ActualEventList from '@/components/ActualEvent/ActualEventList.vue';
import ActualEventForm from '@/components/ActualEvent/ActualEventForm.vue';

const routes = [
  {
    path: '/home',
    name: 'PlannedEventList',
    component: PlannedEventList
  },
  {
    path: '/add/plannedevent',
    name: 'PlannedEventForm',
    component: PlannedEventForm
  },
  {
    path: '/actualevents',
    name: 'ActualEventList',
    component: ActualEventList
  },
  {
    path: '/add/actualevent',
    name: 'ActualEventForm',
    component: ActualEventForm
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

export default router;
