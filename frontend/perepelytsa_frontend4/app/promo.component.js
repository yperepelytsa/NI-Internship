"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require('@angular/core');
var PromoComponent = (function () {
    function PromoComponent() {
    }
    PromoComponent.prototype.countsum = function (inp) {
        var res = 0;
        inp.forEach(function (element) {
            res += element.quantity * element.itemPrice;
        });
        return res;
    };
    PromoComponent.prototype.counttotal = function () {
        if (this.promo == null)
            return this.countsum(this.itemsinp);
        if (this.promo.includes("ni16%")) {
            return Math.round(this.countsum(this.itemsinp) * 0.84);
        }
        if (this.promo.includes("ni33%")) {
            return Math.round(this.countsum(this.itemsinp) * 0.67);
        }
        if (this.promo.includes("ni10$")) {
            if (this.countsum(this.itemsinp) > 10)
                return this.countsum(this.itemsinp) - 10;
            else
                return 0;
        }
        return this.countsum(this.itemsinp);
    };
    __decorate([
        core_1.Input(), 
        __metadata('design:type', Array)
    ], PromoComponent.prototype, "itemsinp", void 0);
    PromoComponent = __decorate([
        core_1.Component({
            selector: 'my-promo',
            template: "\t\t\t\n     Retail price: {{countsum(itemsinp)}}$ <br>\n     Promocode:<br>\n     <input [(ngModel)]=\"promo\" class=\"input-promo bg-10\"/><br>\n     <div [ngSwitch]=\"promo\">\n     <div *ngSwitchCase=\"'ni16%'\">Discount: 16%</div>\n     <div *ngSwitchCase=\"'ni33%'\">Discount: 33%</div>\n     <div *ngSwitchCase=\"'ni10$'\">Discount: 10$</div>\n     <div *ngSwitchDefault></div>\n    </div>\n     <b>Total: {{counttotal(itemsinp)}}$</b><br>\n     <button type=\"button\" id=\"checkout\" class=\"btn btn-default btn-sm\">Checkout</button>\n  " }), 
        __metadata('design:paramtypes', [])
    ], PromoComponent);
    return PromoComponent;
}());
exports.PromoComponent = PromoComponent;
//# sourceMappingURL=promo.component.js.map