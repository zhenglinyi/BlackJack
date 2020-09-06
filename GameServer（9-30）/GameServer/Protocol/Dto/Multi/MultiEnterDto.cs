using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Dto.Multi
{
    [Serializable]
    public class MultiEnterDto
    {
        public UserDto userDto;
        public int position;

        public MultiEnterDto(UserDto userDto, int position)
        {
            this.userDto = userDto;
            this.position = position;
        }
    }
}
