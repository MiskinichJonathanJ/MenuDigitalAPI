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
async function retryWithBackoff(fn, retries = 3, delay = 500) {
    try {
        return await fn();
    } catch (error) {
        if (retries <= 0) throw error;
        await new Promise(res => setTimeout(res, delay));
        return retryWithBackoff(fn, retries - 1, delay * 2);
    }
}

export const apiRequest = async (url, options = {}) => {
    const config = {
        timeout: 10000,
        retries: 3,
        retryDelay: 1000,
        ...options
    };

    return retryWithBackoff(
        () => fetchWithTimeout(url, config),
        config.retries,
        config.retryDelay
    );
};

async function fetchWithTimeout(url, config) {
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), config.timeout);

    try {
        const response = await fetch(url, {
            ...config,
            signal: controller.signal,
            headers: { ...API_CONFIG.HEADERS, ...config.headers }
        });

        clearTimeout(timeoutId);

        if (!response.ok) {
            throw new ApiError(response.status, await getErrorMessage(response));
        }

        return await response.json();
    } catch (error) {
        clearTimeout(timeoutId);
        throw normalizeError(error);
    }
}