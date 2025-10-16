import { buildApiUrl, apiRequest, API_CONFIG } from '../config/api.js';
import { showError } from '../utils/helpers.js';

const StatusService = {
    async getAllStatuses() {
        try {
            const url = buildApiUrl(API_CONFIG.ENDPOINTS.STATUS);
            return await apiRequest(url, { method: 'GET' });
        } catch (error) {
            showError('Error al cargar los estados');
            throw error;
        }
    },

    async getStatusMap() {
        try {
            const statuses = await this.getAllStatuses();
            const map = {};
            statuses.forEach(s => {
                map[s.id] = s.name;
            });
            return map;
        } catch (error) {
            console.error('Error al crear el mapa de estados:', error);
            return {};
        }
    }
};

export { StatusService };
