import { Component, OnDestroy, OnInit } from "@angular/core";
import { Subject, takeUntil } from "rxjs";
import { Toast, ToastService } from "./toasts.service";

@Component({
    selector: 'app-toasts',
    templateUrl: './toasts.component.html',
    styleUrls: ['./toasts.component.css'],
})
export class ToastsComponent implements OnInit, OnDestroy {
    private ngUnsub = new Subject<void>();
    toasts: Toast[] = [];

    constructor(private toastService: ToastService) { }

    ngOnInit() {
        this.toastService.toasts$
            .pipe(takeUntil(this.ngUnsub))
            .subscribe(t => this.toasts = t);
    }

    ngOnDestroy() {
        this.ngUnsub.next();
        this.ngUnsub.complete();
        this.toastService.clear();
    }
}