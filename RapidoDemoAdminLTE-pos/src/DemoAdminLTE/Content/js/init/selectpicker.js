SelectPicker = {
    init: function () {
        var lang = document.documentElement.lang;

        if ($.fn.selectpicker) {
            switch (lang) {
                case 'en':
                    $.fn.selectpicker.defaults = {
                        noneSelectedText: 'Nothing selected',
                        noneResultsText: 'No results match {0}',
                        countSelectedText: function (numSelected, numTotal) {
                            return (numSelected == 1) ? '{0} item selected' : '{0} items selected';
                        },
                        maxOptionsText: function (numAll, numGroup) {
                            return [
                                (numAll == 1) ? 'Limit reached ({n} item max)' : 'Limit reached ({n} items max)',
                                (numGroup == 1) ? 'Group limit reached ({n} item max)' : 'Group limit reached ({n} items max)'
                            ];
                        },
                        selectAllText: 'Select All',
                        deselectAllText: 'Deselect All',
                        multipleSeparator: ', '
                    };
                    break;
                case 'vi':
                    $.fn.selectpicker.defaults = {
                        noneSelectedText: 'Chưa chọn',
                        noneResultsText: 'Không có kết quả cho {0}',
                        countSelectedText: function (numSelected, numTotal) {
                            return '{0} mục đã chọn';
                        },
                        maxOptionsText: function (numAll, numGroup) {
                            return [
                                'Không thể chọn (giới hạn {n} mục)',
                                'Không thể chọn (giới hạn {n} mục)'
                            ];
                        },
                        selectAllText: 'Chọn tất cả',
                        deselectAllText: 'Bỏ chọn',
                        multipleSeparator: ', '
                    };
                    break;
            }

            $('select.apply_selectpicker').selectpicker();

        }
    }
};
