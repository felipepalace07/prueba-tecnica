import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ResultadoProceso, Resumen } from '../models/factura.models';
import { apiUrl } from '../env';

@Injectable({ providedIn: 'root' })
export class FacturasService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${apiUrl}/facturas`;

  obtenerResumen(): Observable<Resumen> {
    return this.http.get<Resumen>(`${this.baseUrl}/resumen`);
  }

  procesarRecordatorios(): Observable<ResultadoProceso> {
    return this.http.post<ResultadoProceso>(`${this.baseUrl}/procesar-recordatorios`, {});
  }

  reiniciarDatos(): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/reiniciar-datos`, {});
  }
}
