export type EstadoFactura = 'Pendiente' | 'PrimerRecordatorio' | 'SegundoRecordatorio' | 'Desactivado';

export interface FacturaResumen {
  clienteId: string;
  cliente: string;
  email: string;
  numero: string;
  valor: number;
  fechaVencimiento: string;
  estado: EstadoFactura;
}

export interface Resumen {
  facturas: FacturaResumen[];
}

export interface ResultadoFactura {
  cliente: string;
  numeroFactura: string;
  estadoAnterior: EstadoFactura;
  estadoNuevo: EstadoFactura | null;
  exitoso: boolean;
  error: string | null;
}

export interface ResultadoProceso {
  resultados: ResultadoFactura[];
  procesadas: number;
  exitosas: number;
  fallidas: number;
}

export const ESTADOS: ReadonlyArray<{ valor: EstadoFactura; etiqueta: string }> = [
  { valor: 'Pendiente', etiqueta: 'Pendiente' },
  { valor: 'PrimerRecordatorio', etiqueta: 'Primer recordatorio' },
  { valor: 'SegundoRecordatorio', etiqueta: 'Segundo recordatorio' },
  { valor: 'Desactivado', etiqueta: 'Desactivado' },
];
