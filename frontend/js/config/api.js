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

class ApiError extends Error {
    constructor(status, message) {
        super(message);
        this.name = 'ApiError';
        this.status = status;
    }
}

async function getErrorMessage(response) {
    try {
        const data = await response.json();
        return data.message || data.error || `Error ${response.status}`;
    } catch {
        return `Error ${response.status}: ${response.statusText}`;
    }
}

function normalizeError(error) {
    if (error.name === 'AbortError') {
        return new Error('La petición tardó demasiado tiempo');
    }
    if (error instanceof ApiError) {
        return error;
    }
    return new Error(error.message || 'Error de red');
}

async function retryWithBackoff(fn, retries = 3, delay = 500) {
    try {
        return await fn();
    } catch (error) {
        if (retries <= 0) throw error;
        await new Promise(res => setTimeout(res, delay));
        return retryWithBackoff(fn, retries - 1, delay * 2);
    }
}

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
            const errorMsg = await getErrorMessage(response);
            throw new ApiError(response.status, errorMsg);
        }

        return await response.json();
    } catch (error) {
        clearTimeout(timeoutId);
        throw normalizeError(error);
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