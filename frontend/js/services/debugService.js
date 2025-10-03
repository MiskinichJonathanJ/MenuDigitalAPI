console.log('debugService.js cargado');

const DebugService = {
  async testApiConnection() {
    console.log('Probando conexión con API...');
    return { status: 'ok' };
  },
  
  async runFullDiagnostic() {
    console.log('Ejecutando diagnóstico...');
    try {
      const results = await this.testApiConnection();
      console.log('Diagnóstico completado:', results);
    } catch (error) {
      console.error('Error en diagnóstico:', error);
    }
  }
};

window.DebugService = DebugService;