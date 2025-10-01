console.log('debugService.js cargado');

const DebugService = {
  async testApiConnection() {
    console.log('Probando conexi�n con API...');
    // Implementaci�n simple por ahora
    return { status: 'ok' };
  },
  
  async runFullDiagnostic() {
    console.log('Ejecutando diagn�stico...');
    try {
      const results = await this.testApiConnection();
      console.log('Diagn�stico completado:', results);
    } catch (error) {
      console.error('Error en diagn�stico:', error);
    }
  }
};

window.DebugService = DebugService;