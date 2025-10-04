export const API_CONFIG = {
  BASE_URL: 'http://localhost:5000/api/v1',
  ENDPOINTS: {
    DISHES: '/Dish',
    CATEGORIES: '/Category',
    ORDERS: '/Order',
    STATUS: '/Status',
    DELIVERY_TYPES: '/DeliveryType'
  },
  HEADERS: {
      'Content-Type': 'application/json; charset=utf-8',
      'Accept': 'application/json; charset=utf-8'
  }
};

export function buildApiUrl(endpoint, params = '') {
    return `${API_CONFIG.BASE_URL}${endpoint}${params}`;
}

const handleApiError = async (response) => {
  if (!response.ok) {
    const errorData = await response.json().catch(() => ({ message: 'Error desconocido' }));
    throw new Error(errorData.message || `Error ${response.status}: ${response.statusText}`);
  }
  return response;
};

export const apiRequest = async (url, options = {}) => {
  try {
    const response = await fetch(url, {
      ...options,
      headers: {
        ...API_CONFIG.HEADERS,
        ...options.headers
      }
    });
    
    await handleApiError(response);
    return await response.json();
  } catch (error) {
    throw error;
  }
};