/***************************************************************************************************
 * Load `$localize` onto the global scope - used if i18n tags appear in Angular templates.
 */
import '@angular/localize/init';
import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';

export function getApiBaseUrl(): string {
  return 'https://localhost:6100/';
}
export function getApiUrl(): string {
  return 'https://localhost:6100/api/v1.0';
}
export function getIdpBaseUrl(): string {
  return 'https://localhost:5100';
}
export function getIdpApiUrl(): string {
  return 'https://localhost:5100/api/v1.0';
}

const providers = [
  { provide: 'API_BASE_URL', useFactory: getApiBaseUrl, deps: [] },
  { provide: 'API_URL', useFactory: getApiUrl, deps: [] },
  { provide: 'IDP_API_URL', useFactory: getIdpApiUrl, deps: [] },
  { provide: 'IDP_BASE_URL', useFactory: getIdpBaseUrl, deps: [] }
];

if (environment.production) {
  enableProdMode();
}

platformBrowserDynamic(providers).bootstrapModule(AppModule)
  .catch(err => console.log(err));
