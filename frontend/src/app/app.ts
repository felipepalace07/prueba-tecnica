import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { Observable, finalize } from 'rxjs';
import { environment } from '../environments/environment';
import { ESTADOS, EstadoFactura, ResultadoProceso, Resumen } from './models/factura.models';
import { FacturasService } from './services/facturas.service';

@Component({
  selector: 'app-root',
  imports: [CurrencyPipe, DatePipe],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  private readonly facturasService = inject(FacturasService);

  protected readonly mailpitUrl = environment.mailpitUrl;
  protected readonly resumen = signal<Resumen | null>(null);
  protected readonly resultado = signal<ResultadoProceso | null>(null);
  protected readonly cargando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly clienteSeleccionado = signal('');

  protected readonly clientes = computed(() => {
    const facturas = this.resumen()?.facturas ?? [];
    const unicos = new Map(facturas.map(f => [f.clienteId, f.cliente]));
    return [...unicos].map(([id, nombre]) => ({ id, nombre }));
  });

  protected readonly facturasFiltradas = computed(() => {
    const facturas = this.resumen()?.facturas ?? [];
    const clienteId = this.clienteSeleccionado();
    return clienteId ? facturas.filter(f => f.clienteId === clienteId) : facturas;
  });

  protected readonly totales = computed(() =>
    ESTADOS.map(estado => {
      const facturas = this.facturasFiltradas().filter(f => f.estado === estado.valor);
      return {
        ...estado,
        cantidad: facturas.length,
        monto: facturas.reduce((suma, f) => suma + f.valor, 0),
      };
    }),
  );

  ngOnInit(): void {
    this.cargarResumen();
  }

  protected etiqueta(estado: EstadoFactura | null): string {
    return ESTADOS.find(e => e.valor === estado)?.etiqueta ?? '—';
  }

  protected seleccionarCliente(evento: Event): void {
    this.clienteSeleccionado.set((evento.target as HTMLSelectElement).value);
  }

  protected procesar(): void {
    this.ejecutar(this.facturasService.procesarRecordatorios(), resultado => this.resultado.set(resultado));
  }

  protected reiniciar(): void {
    this.ejecutar(this.facturasService.reiniciarDatos(), () => this.resultado.set(null));
  }

  private cargarResumen(): void {
    this.facturasService.obtenerResumen().subscribe({
      next: resumen => {
        this.resumen.set(resumen);
        this.error.set(null);
      },
      error: () => this.error.set('No se pudo conectar con la API. Verifica que esté corriendo en el puerto 5080.'),
    });
  }

  private ejecutar<T>(accion: Observable<T>, alTerminar: (valor: T) => void): void {
    this.cargando.set(true);
    accion.pipe(finalize(() => this.cargando.set(false))).subscribe({
      next: valor => {
        alTerminar(valor);
        this.cargarResumen();
      },
      error: () => this.error.set('Ocurrió un error al comunicarse con la API.'),
    });
  }
}

