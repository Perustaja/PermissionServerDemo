import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})

export class ToastService {
    private toasts: Toast[] = [];
    private toastsSubject = new Subject<Toast[]>();
    toasts$ = this.toastsSubject.asObservable();

    pushSuccess(msg: string) {
        this.toasts = [this.createToast('bg-success text-white', msg), ...this.toasts];
        this.toastsSubject.next(this.toasts);
    }

    pushWarning(msg: string) {
        this.toasts = [this.createToast('bg-warning text-dark', msg), ...this.toasts];
        this.toastsSubject.next(this.toasts);
    }

    pushDanger(msg: string) {
        this.toasts = [this.createToast('bg-danger text-white', msg), ...this.toasts];
        this.toastsSubject.next(this.toasts);
    }

    clear() {
        this.toasts = [];
        this.toastsSubject.next(this.toasts);
    }

    private createToast(classType: string, msg: string): Toast {
        return <Toast>({classType: classType, message: msg});
    }
}

export interface Toast {
    classType: string;
    message: string;
}
  