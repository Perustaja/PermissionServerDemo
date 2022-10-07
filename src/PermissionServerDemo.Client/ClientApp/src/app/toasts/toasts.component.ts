import { Component, OnDestroy } from "@angular/core";
import { Toast, ToastService } from "./toasts.service";

@Component({
    selector: 'app-toasts',
    templateUrl: './toasts.component.html',
    styleUrls: ['./toasts.component.css'],
})
export class ToastsComponent implements OnDestroy {
    toasts: Toast[] = [];

    constructor(private toastService: ToastService) {
        toastService.toasts$.subscribe(t => this.toasts = t);
    }

    ngOnDestroy(): void {
        this.toastService.clear();
    }
}